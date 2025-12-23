using AMMS.Application.Interfaces;
using AMMS.Infrastructure.DBContext;
using AMMS.Infrastructure.Entities;
using AMMS.Shared.DTOs.Materials;
using AMMS.Shared.DTOs.Purchases;
using Microsoft.EntityFrameworkCore;

namespace AMMS.Application.Services
{
    public class MaterialPurchaseRequestService : IMaterialPurchaseRequestService
    {
        private readonly AppDbContext _db;

        public MaterialPurchaseRequestService(AppDbContext db)
        {
            _db = db;
        }

        // Tính thiếu NVL cho 1 order
        private async Task<List<MaterialShortageDto>> GetShortagesForOrderAsync(
            int orderId,
            CancellationToken ct)
        {
            // Join order_items -> boms -> materials
            var raw = await (
                from oi in _db.order_items
                join b in _db.boms on oi.item_id equals b.order_item_id
                join m in _db.materials on b.material_id equals m.material_id
                where oi.order_id == orderId
                select new
                {
                    OrderQty = (decimal)oi.quantity,
                    BomQtyPerProduct = b.qty_per_product ?? 0m,
                    WastagePercent = b.wastage_percent ?? 0m,
                    Material = m
                }
            ).ToListAsync(ct);

            if (!raw.Any())
                return new List<MaterialShortageDto>();

            var grouped = raw
    .GroupBy(x => x.Material.material_id)
    .Select(g =>
    {
        var m = g.First().Material;

        // Tổng nhu cầu = sum(quantity * qty_per_product * (1 + waste%))
        decimal required = g.Sum(x =>
        {
            var factor = 1m + (x.WastagePercent / 100m);
            return x.OrderQty * x.BomQtyPerProduct * factor;
        });

        decimal stock = m.stock_qty ?? 0m;
        decimal shortage = required > stock ? required - stock : 0m;

        return new MaterialShortageDto
        {
            MaterialId = m.material_id,
            Code = m.code,
            Name = m.name,
            Unit = m.unit,
            StockQty = stock,
            RequiredQty = required,
            ShortageQty = shortage,
            // mặc định số lượng đề xuất mua = số lượng thiếu
            NeedToBuyQty = shortage
        };
    })
    .Where(x => x.ShortageQty > 0m)
    .ToList();

            return grouped;
        }

        // Sinh mã phiếu mua: PO-0001, PO-0002,...
        private async Task<string> GenerateNextPurchaseCodeAsync(CancellationToken ct)
        {
            var lastCode = await _db.purchases.AsNoTracking()
                .OrderByDescending(p => p.purchase_id)
                .Select(p => p.code)
                .FirstOrDefaultAsync(ct);

            int nextNum = 1;
            if (!string.IsNullOrWhiteSpace(lastCode))
            {
                var digits = new string(lastCode.Where(char.IsDigit).ToArray());
                if (int.TryParse(digits, out var n))
                    nextNum = n + 1;
            }

            return $"PO-{nextNum:0000}";
        }

        public async Task<AutoPurchaseResultDto> CreateFromOrderAsync(
    int orderId,
    int managerId,
    CancellationToken ct = default)
        {
            // Check order tồn tại
            var order = await _db.orders
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.order_id == orderId, ct);

            if (order == null)
                throw new KeyNotFoundException("Order not found");

            // 1) Tính thiếu NVL
            var shortages = await GetShortagesForOrderAsync(orderId, ct);
            if (!shortages.Any())
                throw new InvalidOperationException("Đơn hàng không thiếu nguyên vật liệu.");

            // 2) Tạo purchase header (chưa chọn supplier)
            var code = await GenerateNextPurchaseCodeAsync(ct);

            var purchase = new purchase
            {
                code = code,
                supplier_id = null,      // manager sẽ chọn sau
                created_by = managerId,
                status = "Pending",
                eta_date = null,
                created_at = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
            };

            await _db.purchases.AddAsync(purchase, ct);
            await _db.SaveChangesAsync(ct); // để có purchase_id

            // ===== 3) Tạo chi tiết từng NVL thiếu (MUA DƯ) =====
            const decimal bufferPercent = 0.30m; // mua dư 30%

            foreach (var s in shortages)
            {
                // nếu không thiếu thì bỏ qua
                if (s.ShortageQty <= 0) continue;

                // mua dư: NeedToBuy = Shortage * (1 + buffer%)
                var buyQty = s.ShortageQty * (1 + bufferPercent);

                // làm tròn 2 chữ số thập phân (tùy DB)
                buyQty = decimal.Round(buyQty, 2, MidpointRounding.AwayFromZero);

                // cập nhật lại vào DTO cho FE / manager thấy
                s.NeedToBuyQty = buyQty;

                var item = new purchase_item
                {
                    purchase_id = purchase.purchase_id,
                    material_id = s.MaterialId,
                    qty_ordered = buyQty,   // ✅ dùng NeedToBuyQty thay vì ShortageQty
                    price = 0m        // manager chỉnh sau
                };

                await _db.purchase_items.AddAsync(item, ct);
            }

            await _db.SaveChangesAsync(ct);

            return new AutoPurchaseResultDto
            {
                PurchaseId = purchase.purchase_id,
                PurchaseCode = purchase.code!,
                Items = shortages  // lúc này mỗi item đã có NeedToBuyQty > ShortageQty
            };
        }
    }
}
