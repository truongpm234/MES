using AMMS.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMMS.Infrastructure.Interfaces
{
    public interface IProductionRepository
    {
        Task<List<production>> GetProductionsInProgressAsync();
        Task<DateTime?> GetNearestDeliveryDateAsync();
    }
}
