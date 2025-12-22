using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMMS.Infrastructure.Entities;

namespace AMMS.Infrastructure.Interfaces
{
    public interface IQuoteRepository
    {
        Task AddAsync(quote entity);
        Task SaveChangesAsync();
    }
}

