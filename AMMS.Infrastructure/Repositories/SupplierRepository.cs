using AMMS.Infrastructure.DBContext;
using AMMS.Infrastructure.Entities;
using AMMS.Infrastructure.Interfaces;
using AMMS.Shared.DTOs.Common;
using AMMS.Shared.DTOs.Suppliers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMMS.Infrastructure.Repositories
{
    public class SupplierRepository : ISupplierRepository
    {
        private readonly AppDbContext _db;

        public SupplierRepository(AppDbContext db)
        {
            _db = db;
        }

        public Task<int> CountAsync(CancellationToken ct = default)
            => _db.suppliers.AsNoTracking().CountAsync(ct);

        public Task<List<supplier>> GetPagedAsync(int skip, int take, CancellationToken ct = default)
            => _db.suppliers
                .AsNoTracking()
                .Skip(skip)
                .Take(take)
                .ToListAsync(ct);

        public async Task<SupplierDetailDto?> GetSupplierDetailWithMaterialsAsync(
            int supplierId, int page, int pageSize, CancellationToken ct = default)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            // 1) supplier info
            var supplierInfo = await _db.suppliers
                .AsNoTracking()
                .Where(s => s.supplier_id == supplierId)
                .Select(s => new
                {
                    s.supplier_id,
                    s.name,
                    s.contact_person,
                    s.phone,
                    s.email,
                    s.main_material_type
                })
                .FirstOrDefaultAsync(ct);

            if (supplierInfo == null) return null;

            var skip = (page - 1) * pageSize;

            // 2) materials by purchase history: supplier -> purchases -> purchase_items -> materials
            // IMPORTANT: OrderBy on scalar (TotalQty) BEFORE projecting to DTO
            var baseQuery =
                from p in _db.purchases.AsNoTracking()
                join pi in _db.purchase_items.AsNoTracking() on p.purchase_id equals pi.purchase_id
                join m in _db.materials.AsNoTracking() on pi.material_id equals m.material_id
                where p.supplier_id == supplierId
                group pi by new { m.material_id, m.code, m.name, m.unit } into g
                select new
                {
                    g.Key.material_id,
                    g.Key.code,
                    g.Key.name,
                    g.Key.unit,
                    TotalQty = g.Sum(x => x.qty_ordered) // decimal?
                };

            var orderedQuery = baseQuery
                .OrderByDescending(x => x.TotalQty)
                .ThenBy(x => x.name);

            var list = await orderedQuery
                .Skip(skip)
                .Take(pageSize + 1)
                .Select(x => new SupplierMaterialDto(
                    x.material_id,
                    x.code,
                    x.name,
                    x.unit,
                    x.TotalQty ?? 0m
                ))
                .ToListAsync(ct);

            var hasNext = list.Count > pageSize;
            if (hasNext) list = list.Take(pageSize).ToList();

            var pagedMaterials = new PagedResultLite<SupplierMaterialDto>
            {
                Page = page,
                PageSize = pageSize,
                HasNext = hasNext,
                Data = list
            };

            return new SupplierDetailDto(
                supplierInfo.supplier_id,
                supplierInfo.name,
                supplierInfo.contact_person,
                supplierInfo.phone,
                supplierInfo.email,
                supplierInfo.main_material_type,
                pagedMaterials
            );
        }
    }
}
