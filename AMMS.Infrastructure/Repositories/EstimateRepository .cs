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
    public class EstimateRepository : IEstimateRepository
    {
        private readonly AppDbContext _db;
        public EstimateRepository(AppDbContext db) => _db = db;

        public async Task<material?> GetMaterialByIdAsync(int id)
        {
            return await _db.materials
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.material_id == id);
        }
    }
}
