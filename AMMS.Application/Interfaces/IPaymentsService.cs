using AMMS.Infrastructure.Entities;

namespace AMMS.Application.Interfaces
{
    public interface IPaymentsService
    {
        Task<payment?> GetPaidByProviderOrderCodeAsync(string provider, int orderCode, CancellationToken ct = default);
    }
}