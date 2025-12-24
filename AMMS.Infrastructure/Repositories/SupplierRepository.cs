using AMMS.Infrastructure.DBContext;
using AMMS.Infrastructure.Entities;
using AMMS.Infrastructure.Interfaces;
using AMMS.Shared.DTOs.Common;
using AMMS.Shared.DTOs.Suppliers;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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

        // ✅ Lấy danh sách supplier + materials có main_material_type trùng nhau
        public async Task<List<SupplierLiteDto>> GetPagedAsync(
    int skip, int take, CancellationToken ct = default)
        {
            return await _db.suppliers
                .AsNoTracking()
                .OrderBy(s => s.name)
                .Skip(skip)
                .Take(take)
                .Select(s => new SupplierLiteDto
                {
                    SupplierId = s.supplier_id,
                    Name = s.name,
                    ContactPerson = s.contact_person,
                    Phone = s.phone,
                    Email = s.email,
                    MainMaterialType = s.main_material_type,
                    Rating = s.rating 
                })
                .ToListAsync(ct);
        }


        public async Task<SupplierDetailDto?> GetSupplierDetailWithMaterialsAsync(
    int supplierId, int page, int pageSize, CancellationToken ct = default)
        {
            page = page <= 0 ? 1 : page;
            pageSize = pageSize <= 0 ? 10 : pageSize;

            var supplier = await _db.suppliers.AsNoTracking()
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

            if (supplier == null) return null;

            // ✅ Query base: select ra anonymous trước (EF dịch SQL được)
            var baseQuery =
                from sm in _db.supplier_materials.AsNoTracking()
                where sm.supplier_id == supplierId
                join m in _db.materials.AsNoTracking()
                    on sm.material_id equals m.material_id
                select new
                {
                    m.material_id,
                    m.code,
                    m.name,
                    m.unit,
                    sm.is_active,
                    sm.note
                };

            var totalCount = await baseQuery.CountAsync(ct);

            // ✅ OrderBy trước, rồi mới Select DTO
            var items = await baseQuery
                .OrderBy(x => x.name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new SupplierMaterialDto(
                    x.material_id,
                    x.code,
                    x.name,
                    x.unit,
                    x.is_active,
                    x.note
                ))
                .ToListAsync(ct);

            return new SupplierDetailDto
            {
                supplier_id = supplier.supplier_id,
                name = supplier.name,
                contact_person = supplier.contact_person,
                phone = supplier.phone,
                email = supplier.email,
                main_material_type = supplier.main_material_type,
                Materials = new PagedResultLite<SupplierMaterialDto>
                {
                    Page = page,
                    PageSize = pageSize,
                    HasNext = (page * pageSize) < totalCount,
                    Data = items
                }
            };
        }
    }
}