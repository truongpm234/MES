using AMMS.Infrastructure.Entities;
using AMMS.Shared.DTOs.Orders;

namespace AMMS.Application.Interfaces
{
    public interface IRequestService
    {
        Task<CreateCustomerOrderResponse> CreateAsync(CreateCustomerOrderResquest req);
<<<<<<< HEAD
        Task<UpdateOrderRequestResponse> UpdateAsync(int id, UpdateOrderRequest req);
        Task DeleteAsync(int id);
=======
        //Task UpdateAsync(int id, CreateCustomerOrderResquest req);
        //Task DeleteAsync(int id);
        Task<order_request?> GetByIdAsync(int id);
>>>>>>> b4a30e16b3efda1b30dab78de93abb4548deb28c
    }
}
