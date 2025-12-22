using AMMS.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMMS.Infrastructure.Interfaces
{
    public interface IPurchaseRepository
    {
        Task AddPurchaseAsync(purchase entity, CancellationToken ct = default);
        Task AddPurchaseItemsAsync(IEnumerable<purchase_item> items, CancellationToken ct = default);
        Task<bool> MaterialExistsAsync(int materialId, CancellationToken ct = default);
        Task<int> SaveChangesAsync(CancellationToken ct = default);

        Task<string> GenerateNextPurchaseCodeAsync(CancellationToken ct = default);
    }
}
