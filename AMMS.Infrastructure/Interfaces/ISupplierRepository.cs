using AMMS.Infrastructure.Entities;
using AMMS.Shared.DTOs.Suppliers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMMS.Infrastructure.Interfaces
{
    public interface ISupplierRepository
    {
        Task<int> CountAsync(CancellationToken ct = default);
        Task<List<supplier>> GetPagedAsync(int skip, int take, CancellationToken ct = default);
        Task<SupplierDetailDto?> GetSupplierDetailWithMaterialsAsync(
            int supplierId, int page, int pageSize, CancellationToken ct = default);
    }
}
