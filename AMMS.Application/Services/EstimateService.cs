using AMMS.Application.Interfaces;
using AMMS.Infrastructure.Entities;
using AMMS.Infrastructure.Interfaces;
using AMMS.Shared.DTOs.Estimates;

namespace AMMS.Application.Services
{
    public class EstimateService : IEstimateService
    {
        private readonly IMaterialRepository _materialRepo;
        private readonly ICostEstimateRepository _estimateRepo;

        private const decimal DEFAULT_WASTE_PERCENT = 5m;

        public EstimateService(IMaterialRepository materialRepo, ICostEstimateRepository costEstimateRepository)
        {
            _materialRepo = materialRepo;
            _estimateRepo = costEstimateRepository;
        }

        public async Task<PaperEstimateResponse> EstimatePaperAsync(PaperEstimateRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.paper_code))
                throw new ArgumentException("paper_code is required");

            if (req.quantity <= 0)
                throw new ArgumentException("quantity must be > 0");

            if (req.length_mm < 0 || req.width_mm < 0 || req.height_mm < 0)
                throw new ArgumentException("length_mm/width_mm/height_mm must be >= 0");

            if (req.allowance_mm < 0 || req.bleed_mm < 0)
                throw new ArgumentException("allowance_mm/bleed_mm must be >= 0");

            var paper = await _materialRepo.GetByCodeAsync(req.paper_code)
                       ?? throw new KeyNotFoundException($"Paper not found: {req.paper_code}");

            if (paper.sheet_width_mm is null || paper.sheet_height_mm is null || paper.sheet_length_mm is null)
                throw new InvalidOperationException("Missing sheet size in materials (sheet_width_mm/sheet_height_mm/sheet_length_mm).");

            int sheetW = paper.sheet_width_mm.Value;
            int sheetH = paper.sheet_height_mm.Value;
            int sheetL = paper.sheet_length_mm.Value;

            // print size (hộp cơ bản - mặt trải phẳng của hộp)
            // Chiều rộng in = (chiều dài hộp + chiều rộng hộp) × 2 + chừa gấp + chừa xén 2 bên
            int printW = 2 * (req.length_mm + req.width_mm) + req.allowance_mm + 2 * req.bleed_mm;

            // Chiều cao in = chiều cao hộp + chiều rộng hộp + chừa gấp + chừa xén 2 bên
            int printH = (req.height_mm + req.width_mm) + req.allowance_mm + 2 * req.bleed_mm;

            // Chiều dài in = chiều dài hộp (cho trường hợp tính toán 3D)
            // Với hộp, chiều dài in có thể là phần nắp hộp hoặc phần mở rộng khác
            int printL = req.length_mm + req.allowance_mm + 2 * req.bleed_mm;

            bool fitWH = printW <= sheetW && printH <= sheetH;
            bool fitHW = printH <= sheetW && printW <= sheetH;
            bool fitWL = printW <= sheetW && printL <= sheetL;
            bool fitLW = printL <= sheetW && printW <= sheetL;
            bool fitHL = printH <= sheetW && printL <= sheetL;
            bool fitLH = printL <= sheetW && printH <= sheetL;

            if (!fitWH && !fitHW && !fitWL && !fitLW && !fitHL && !fitLH)
                throw new InvalidOperationException(
                    "Print size does not fit paper in any orientation (W×H, H×W, W×L, L×W, H×L, L×H).");

            // ================== N-up calculation ==================
            int n_wh = fitWH ? (sheetW / printW) * (sheetH / printH) : 0;
            int n_hw = fitHW ? (sheetW / printH) * (sheetH / printW) : 0;

            // Tính theo chiều dài giấy (cuộn / nối)
            int n_wl = fitWL ? (sheetW / printW) * (sheetL / printL) : 0;
            int n_lw = fitLW ? (sheetW / printL) * (sheetL / printW) : 0;
            int n_hl = fitHL ? (sheetW / printH) * (sheetL / printL) : 0;
            int n_lh = fitLH ? (sheetW / printL) * (sheetL / printH) : 0;

            int nUp = Math.Max(
                        Math.Max(n_wh, n_hw),
                        Math.Max(Math.Max(n_wl, n_lw), Math.Max(n_hl, n_lh))
                      );

            if (nUp <= 0)
                nUp = 1;

            // ================== Sheet count ==================
            int sheetsBase = (int)Math.Ceiling(req.quantity / (decimal)nUp);

            // ================== Waste ==================
            var waste = DEFAULT_WASTE_PERCENT;
            int sheetsWithWaste =
                (int)Math.Ceiling(sheetsBase * (1m + waste / 100m));

            return new PaperEstimateResponse
            {
                paper_code = paper.code,
                sheet_width_mm = sheetW,
                sheet_height_mm = sheetH,
                sheet_length_mm = sheetL,
                print_width_mm = printW,
                print_height_mm = printH,
                print_length_mm = printL, // Đã được tính toán
                n_up = nUp,
                quantity = req.quantity,
                sheets_base = sheetsBase,
                sheets_with_waste = sheetsWithWaste,
                waste_percent = waste
            };
        }

        public async Task<CostEstimateResponse> CalculateCostEstimateAsync(CostEstimateRequest req)
        {
            // Validate input
            if (req.paper == null)
                throw new ArgumentException("Paper information is required");

            if (req.paper.sheets_with_waste <= 0)
                throw new ArgumentException("Sheets with waste must be greater than 0");

            // 1️⃣ Lấy vật liệu
            var paper = await _materialRepo.GetByCodeAsync(req.paper.paper_code)
                ?? throw new KeyNotFoundException($"Paper not found: {req.paper.paper_code}");

            if (paper.cost_price == null)
                throw new InvalidOperationException($"Paper cost_price missing for {paper.code}");

            // 2️⃣ Tính giá giấy (đơn vị: VND)
            decimal paperCostPerSheet = paper.cost_price.Value;
            decimal totalPaperCost = req.paper.sheets_with_waste * paperCostPerSheet;

            // 3️⃣ Khấu hao máy móc + NVL khác (10%)
            decimal depreciationAndOther = totalPaperCost * 0.10m;
            decimal baseCost = totalPaperCost + depreciationAndOther;

            // 4️⃣ Ngày hoàn thành dự kiến (5 ngày làm việc)
            var now = DateTime.UtcNow;
            var estimatedFinish = now.AddDays(5);

            // 5️⃣ Xác định có phải rush hay không
            bool isRush = false;
            decimal rushPercent = 0;

            if (req.desired_delivery_date < estimatedFinish)
            {
                TimeSpan timeDifference = estimatedFinish - req.desired_delivery_date;
                int daysDifference = (int)timeDifference.TotalDays;

                // Nếu cần giao sớm hơn 3 ngày so với dự kiến thì tính là rush
                if (daysDifference >= 3)
                {
                    isRush = true;

                    // Tính % rush theo baseCost
                    if (baseCost < 500_000) // Dưới 500k
                        rushPercent = 15;
                    else if (baseCost < 1_000_000) // Từ 500k đến dưới 1 triệu
                        rushPercent = 10;
                    else if (baseCost < 5_000_000) // Từ 1 triệu đến dưới 5 triệu
                        rushPercent = 8;
                    else // Trên 5 triệu
                        rushPercent = 5;
                }
            }

            decimal rushAmount = baseCost * rushPercent / 100;
            decimal systemTotalCost = baseCost + rushAmount;

            // 6️⃣ Chuyển tất cả DateTime về Unspecified cho PostgreSQL
            var entity = new cost_estimate
            {
                order_request_id = req.order_request_id,
                base_cost = Math.Round(baseCost, 2),
                is_rush = isRush,
                rush_percent = rushPercent,
                rush_amount = Math.Round(rushAmount, 2),
                system_total_cost = Math.Round(systemTotalCost, 2),
                estimated_finish_date = DateTime.SpecifyKind(estimatedFinish, DateTimeKind.Unspecified),
                desired_delivery_date = DateTime.SpecifyKind(req.desired_delivery_date, DateTimeKind.Unspecified),
                created_at = DateTime.SpecifyKind(now, DateTimeKind.Unspecified)
            };

            await _estimateRepo.AddAsync(entity);
            await _estimateRepo.SaveChangesAsync();

            // 7️⃣ Trả response - vẫn giữ UTC cho client
            return new CostEstimateResponse
            {
                base_cost = Math.Round(baseCost, 2),
                is_rush = isRush,
                rush_percent = rushPercent,
                rush_amount = Math.Round(rushAmount, 2),
                system_total_cost = Math.Round(systemTotalCost, 2),
                estimated_finish_date = DateTime.SpecifyKind(estimatedFinish, DateTimeKind.Utc)
            };
        }
    }
}
