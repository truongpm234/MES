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

        Task<PagedResultLite<PurchaseOrderListItemDto>> GetPurchaseOrdersAsync(
            int page, int pageSize, CancellationToken ct = default);

        Task<PurchaseOrderListItemDto> CreatePurchaseOrderAsync(
            CreatePurchaseRequestDto dto,
            CancellationToken ct = default);

        // ✅ CHANGED: receive theo purchaseId
        Task<object> ReceiveAllPendingPurchasesAsync(int purchaseId, CancellationToken ct = default);

        // ✅ CHANGED: pending paging
        Task<PagedResultLite<PurchaseOrderListItemDto>> GetPendingPurchasesAsync(
            int page, int pageSize, CancellationToken ct = default);
    }
}
