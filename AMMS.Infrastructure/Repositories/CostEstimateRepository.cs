using AMMS.Infrastructure.DBContext;
using AMMS.Infrastructure.Entities;
using AMMS.Infrastructure.Interfaces;

namespace AMMS.Infrastructure.Repositories
{
    public class CostEstimateRepository : ICostEstimateRepository
    {
        private readonly AppDbContext _db;

        public CostEstimateRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(cost_estimate entity)
        {
            await _db.cost_estimates.AddAsync(entity);
        }

        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
