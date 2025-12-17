using AMMS.Infrastructure.Entities;
using AMMS.Shared.DTOs.Orders;

namespace AMMS.Application.Interfaces
{
    public interface IRequestService
    {
        Task<CreateCustomerOrderResponse> CreateAsync(CreateCustomerOrderResquest req);
        //Task UpdateAsync(int id, CreateCustomerOrderResquest req);
        //Task DeleteAsync(int id);
        Task<order_request?> GetByIdAsync(int id);
    }
}
