using AMMS.Shared.DTOs.Common;
using AMMS.Shared.DTOs.Purchases;

namespace AMMS.Application.Interfaces
{
    public interface IPurchaseService
    {
        Task<CreatePurchaseRequestResponse> CreatePurchaseRequestAsync(
            CreatePurchaseRequestDto dto,
            int? createdBy,
            CancellationToken ct = default);

        Task<PagedResultLite<PurchaseOrderListItemDto>> GetPurchaseOrdersAsync( // ✅ CHANGED
            int page, int pageSize, CancellationToken ct = default);

        Task<PurchaseOrderListItemDto> CreatePurchaseOrderAsync(
            CreatePurchaseRequestDto dto,
            CancellationToken ct = default);

        // ✅ receive theo purchaseId
        Task<object> ReceiveAllPendingPurchasesAsync(int purchaseId, CancellationToken ct = default);

        // ✅ pending paging (ListItemDto)
        Task<PagedResultLite<PurchaseOrderListItemDto>> GetPendingPurchasesAsync( // ✅ CHANGED
            int page, int pageSize, CancellationToken ct = default);
    }
}
