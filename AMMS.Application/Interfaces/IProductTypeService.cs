using AMMS.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMMS.Application.Interfaces
{
    public interface IProductTypeService
    {
        Task<List<product_type>> GetAllAsync();
        Task<List<string>> GetAllTypeFormGachAsync();
        Task<List<string>> GetAllTypeHop_MauAsync();
        Task<List<string>> GetAllTypeGeneralAsync();
    }
}
