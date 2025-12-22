using AMMS.Infrastructure.DBContext;
using AMMS.Infrastructure.Entities;
using AMMS.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMMS.Infrastructure.Repositories
{
    public class PurchaseRepository : IPurchaseRepository
    {
        private readonly AppDbContext _db;

        public PurchaseRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task AddPurchaseAsync(purchase entity, CancellationToken ct = default)
            => await _db.purchases.AddAsync(entity, ct);

        public async Task AddPurchaseItemsAsync(IEnumerable<purchase_item> items, CancellationToken ct = default)
            => await _db.purchase_items.AddRangeAsync(items, ct);

        public Task<bool> MaterialExistsAsync(int materialId, CancellationToken ct = default)
            => _db.materials.AsNoTracking().AnyAsync(m => m.material_id == materialId, ct);

        public Task<int> SaveChangesAsync(CancellationToken ct = default)
            => _db.SaveChangesAsync(ct);

        public async Task<string> GenerateNextPurchaseCodeAsync(CancellationToken ct = default)
        {
            // ví dụ code: PUR000001, PUR000002...
            // lấy code lớn nhất rồi +1
            var lastCode = await _db.purchases
                .AsNoTracking()
                .Where(p => p.code != null && p.code.StartsWith("PO"))
                .OrderByDescending(p => p.purchase_id)
                .Select(p => p.code!)
                .FirstOrDefaultAsync(ct);

            int next = 1;
            if (!string.IsNullOrWhiteSpace(lastCode))
            {
                var digits = new string(lastCode.SkipWhile(c => !char.IsDigit(c)).ToArray());
                if (int.TryParse(digits, out var n)) next = n + 1;
            }

            return $"PO{next:D4}";
        }
    }
}
