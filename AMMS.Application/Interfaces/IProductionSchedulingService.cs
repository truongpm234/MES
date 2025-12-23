using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMMS.Application.Interfaces
{
    public interface IProductionSchedulingService
    {
        Task<int> ScheduleOrderAsync(int orderId, int productTypeId, int? managerId = null);
    }
}
