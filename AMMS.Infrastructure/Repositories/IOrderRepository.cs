using AMMS.Infrastructure.Entities;

namespace AMMS.Infrastructure.Repositories
{
    public interface IOrderRepository
    {
        Task AddAsync(order_request entity);
        void Update(order_request entity);
        Task<order_request?> GetByIdAsync(int id);
        Task DeleteAsync(int id);
        Task<int> SaveChangesAsync();
    }
}