using AMMS.Infrastructure.DBContext;
using AMMS.Infrastructure.Entities;
using AMMS.Infrastructure.Interfaces;
using AMMS.Shared.DTOs.Orders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMMS.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _db;
        public OrderRepository(AppDbContext db)
        {
            _db = db;
        }
        private static string ToUtcString(DateTime? dt)
        {
            if (dt is null) return "";
            var v = DateTime.SpecifyKind(dt.Value, DateTimeKind.Utc);
            return v.ToString("O"); 
        }
        private static bool IsNotEnoughStatus(string? status)
        {
            if (string.IsNullOrWhiteSpace(status)) return false;

            // tùy hệ bạn đang set status là gì
            return status.Equals("Not Enough", StringComparison.OrdinalIgnoreCase)
                || status.Equals("false", StringComparison.OrdinalIgnoreCase)
                || status.Equals("0", StringComparison.OrdinalIgnoreCase);
        }

        public async Task<List<OrderResponseDto>> GetPagedWithFulfillAsync(int skip, int take, CancellationToken ct = default)
        {
            // 1) Lấy page orders (kèm customer + item đầu)
            var orders = await _db.orders
                .AsNoTracking()
                .OrderByDescending(o => o.order_date)
                .Skip(skip)
                .Take(take)
                .Select(o => new
                {
                    o.order_id,
                    o.code,
                    o.order_date,
                    o.delivery_date,
                    Status = o.status ?? "",

                    CustomerName =
                        (o.customer != null
                            ? (o.customer.company_name ?? o.customer.contact_name ?? "")
                            : ""),

                    FirstItem = o.order_items
                        .OrderBy(i => i.item_id)
                        .Select(i => new
                        {
                            i.product_name,
                            i.product_type_id,
                            i.quantity
                        })
                        .FirstOrDefault()
                })
                .ToListAsync(ct);

            if (orders.Count == 0) return new List<OrderResponseDto>();

            // 2) CHỈ lấy những order có status = false / Not Enough để tính thiếu vật tư
            var orderIdsNeedCalc = orders
                .Where(o => IsNotEnoughStatus(o.Status))
                .Select(o => o.order_id)
                .ToList();

            // Nếu không có order nào "false" => không cần tính missing_materials
            Dictionary<int, List<MissingMaterialDto>> missingByOrder = new();

            if (orderIdsNeedCalc.Count > 0)
            {
                // 2.1) Lấy BOM lines của các order "false"
                var bomLines = await (
                    from oi in _db.order_items.AsNoTracking()
                    join b in _db.boms.AsNoTracking() on oi.item_id equals b.order_item_id
                    join m in _db.materials.AsNoTracking() on b.material_id equals m.material_id
                    where oi.order_id != null && orderIdsNeedCalc.Contains(oi.order_id.Value)
                    select new
                    {
                        OrderId = oi.order_id!.Value,
                        MaterialId = m.material_id,
                        MaterialName = m.name,
                        StockQty = m.stock_qty ?? 0m,

                        // required theo order (định mức + hao hụt)
                        RequiredLine =
                            (decimal)oi.quantity
                            * (b.qty_per_product ?? 0m)
                            * (1m + ((b.wastage_percent ?? 0m) / 100m))
                    }
                ).ToListAsync(ct);

                var today = DateTime.SpecifyKind(DateTime.UtcNow.Date, DateTimeKind.Unspecified);
                var historyStart = DateTime.SpecifyKind(today.AddDays(-30), DateTimeKind.Unspecified);
                var historyEndExclusive = DateTime.SpecifyKind(today.AddDays(1), DateTimeKind.Unspecified);


                var materialIds = bomLines.Select(x => x.MaterialId).Distinct().ToList();

                var usageLast30List = await _db.stock_moves
                    .AsNoTracking()
                    .Where(s =>
                        s.type == "OUT" &&
                        s.move_date >= historyStart &&
                        s.move_date <= historyEndExclusive &&
                        s.material_id != null &&
                        materialIds.Contains(s.material_id.Value))
                    .GroupBy(s => s.material_id!.Value)
                    .Select(g => new
                    {
                        MaterialId = g.Key,
                        UsageLast30Days = g.Sum(x => x.qty ?? 0m)
                    })
                    .ToListAsync(ct);

                var usageDict = usageLast30List.ToDictionary(x => x.MaterialId, x => x.UsageLast30Days);

                // 2.3) Group theo (order, material) và tính thiếu theo shortage-for-orders:
                // safety = usage30 * 30%
                // needed = required + safety
                // missing = max(0, needed - stock)
                missingByOrder = bomLines
                    .GroupBy(x => new { x.OrderId, x.MaterialId, x.MaterialName, x.StockQty })
                    .Select(g =>
                    {
                        var requiredQty = g.Sum(x => x.RequiredLine);

                        usageDict.TryGetValue(g.Key.MaterialId, out var usage30);
                        var safetyQty = usage30 * 0.30m;

                        var needed = requiredQty + safetyQty;
                        var missing = needed - g.Key.StockQty;
                        if (missing < 0m) missing = 0m;

                        return new
                        {
                            g.Key.OrderId,
                            g.Key.MaterialId,
                            g.Key.MaterialName,
                            Available = g.Key.StockQty,
                            Needed = needed,
                            Missing = missing
                        };
                    })
                    .Where(x => x.Missing > 0m)
                    .GroupBy(x => x.OrderId)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(x => new MissingMaterialDto
                        {
                            material_id = x.MaterialId.ToString(),
                            material_name = x.MaterialName,
                            needed = Math.Round(x.Needed, 4),
                            missing = Math.Round(x.Missing, 4),
                            available = Math.Round(x.Available, 4)
                        }).ToList()
                    );
            }

            // 3) Build response: nếu status=false thì attach missing_materials
            string ToUtcString(DateTime? dt)
            {
                if (dt is null) return "";
                var v = DateTime.SpecifyKind(dt.Value, DateTimeKind.Utc);
                return v.ToString("O");
            }

            return orders.Select(o =>
            {
                bool canFulfill =
                    o.Status.Equals("New", StringComparison.OrdinalIgnoreCase) ? true :
                    IsNotEnoughStatus(o.Status) ? false :
                    true; 

                missingByOrder.TryGetValue(o.order_id, out var missingMaterials);

                return new OrderResponseDto
                {
                    order_id = o.order_id.ToString(),
                    code = o.code,
                    customer_name = o.CustomerName,
                    product_name = o.FirstItem?.product_name,
                    product_id = o.FirstItem?.product_type_id?.ToString(),
                    quantity = o.FirstItem?.quantity ?? 0,
                    created_at = ToUtcString(o.order_date),
                    delivery_date = ToUtcString(o.delivery_date),
                    can_fulfill = canFulfill,
                    missing_materials = canFulfill == false ? (missingMaterials ?? new List<MissingMaterialDto>()) : null
                };
            }).ToList();
        }

        public async Task AddOrderAsync(order entity)
        {
            await _db.orders.AddAsync(entity);
        }
        public void Update(order entity)
        {
            _db.orders.Update(entity);
        }
        public async Task<order?> GetByIdAsync(int id)
        {
            return await _db.orders.FindAsync(id);
        }
        public Task<int> CountAsync()
        {
            return _db.orders.AsNoTracking().CountAsync();
        }

        public Task<List<OrderListDto>> GetPagedAsync(int skip, int take)
        {
            return _db.orders
                .AsNoTracking()
                .OrderByDescending(o => o.order_date)
                .Skip(skip)
                .Take(take)
                .Select(o => new OrderListDto
                {
                    Order_id = o.order_id,
                    Code = o.code,
                    Order_date = o.order_date,
                    Delivery_date = o.delivery_date,
                    Status = o.status,
                    Payment_status = o.payment_status,
                    Quote_id = o.quote_id,
                    Total_amount = o.total_amount
                })
                .ToListAsync();
        }
        public async Task<order?> GetByCodeAsync(string code)
        {
            return await _db.orders
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.code == code);
        }
        public async Task DeleteAsync(int id)
        {
            var order = await GetByIdAsync(id);
            if (order != null)
            {
                _db.orders.Remove(order);
            }
        }
        public async Task<int> SaveChangesAsync()
        {
            return await _db.SaveChangesAsync();
        }
        public Task AddOrderItemAsync(order_item entity) => _db.order_items.AddAsync(entity).AsTask();
        public async Task<string> GenerateNextOrderCodeAsync()
        {
            var last = await _db.orders.AsNoTracking()
                .OrderByDescending(x => x.order_id)
                .Select(x => x.code)
                .FirstOrDefaultAsync();

            int nextNum = 1;
            if (!string.IsNullOrWhiteSpace(last))
            {
                var digits = new string(last.Where(char.IsDigit).ToArray());
                if (int.TryParse(digits, out var n)) nextNum = n + 1;
            }

            return $"ORD-{nextNum:00}";
        }
    }
}

