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
    public class ProductTypeRepository : IProductTypeRepository
    {
        private readonly AppDbContext _db;
        public ProductTypeRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<product_type>> GetAllAsync()
        {
            return await Task.FromResult(_db.product_types.ToList());

        }
        public Task<product_type?> GetByCodeAsync(string code)
        => _db.product_types.FirstOrDefaultAsync(x => x.code == code);
    }
}
