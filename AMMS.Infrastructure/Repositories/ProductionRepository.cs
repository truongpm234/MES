using AMMS.Infrastructure.DBContext;
using AMMS.Infrastructure.Entities;
using AMMS.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMMS.Infrastructure.Repositories
{
    public class ProductionRepository : IProductionRepository
    {
        private readonly AppDbContext _db;

        public ProductionRepository(AppDbContext db)
        {
            _db = db;
        }
        /// <summary>
        /// Ngày giao gần nhất của các đơn đang sản xuất
        /// </summary>
        public async Task<DateTime?> GetNearestDeliveryDateAsync()
        {
            return await _db.productions
                .AsNoTracking()
                .Include(p => p.order)
                .Where(p =>
                    (EF.Functions.ILike(p.status!, "scheduled") ||
                     EF.Functions.ILike(p.status!, "in_production")) &&
                    p.order!.delivery_date != null
                )
                .OrderBy(p => p.order!.delivery_date)
                .Select(p => p.order!.delivery_date)
                .FirstOrDefaultAsync(); 
        }
        public Task AddAsync(production p)
        {
            _db.productions.Add(p);
            return Task.CompletedTask;
        }

        public Task SaveChangesAsync() => _db.SaveChangesAsync();

        public Task<production?> GetByIdAsync(int prodId)
            => _db.productions.FirstOrDefaultAsync(x => x.prod_id == prodId);
    }
}
