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
    public class QuoteRepository : IQuoteRepository
    {
        private readonly AppDbContext _context;

        public QuoteRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(quote entity)
        {
            await _context.quotes.AddAsync(entity);
        }
        public async Task<quote?> GetByIdAsync(int id)
        {
            return await _context.quotes.FirstOrDefaultAsync(x => x.quote_id == id);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
