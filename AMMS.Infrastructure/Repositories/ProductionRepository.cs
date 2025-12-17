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
        /// Danh sách đơn đang sản xuất tại xưởng
        /// </summary>
        public async Task<List<production>> GetProductionsInProgressAsync()
        {
            return await _db.productions
                .AsNoTracking()
                .Include(p => p.order)
                .Where(p =>
                    EF.Functions.ILike(p.status!, "scheduled") ||
                    EF.Functions.ILike(p.status!, "in_production")
                )
                .OrderBy(p => p.order!.delivery_date)
                .ToListAsync();
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

    }
}
