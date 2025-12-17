using AMMS.Application.Interfaces;
using AMMS.Infrastructure.Interfaces;
using AMMS.Shared.DTOs.Productions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMMS.Application.Services
{
    public class ProductionService : IProductionService
    {
        private readonly IProductionRepository _repo;

        public ProductionService(IProductionRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<ProductionOrderDto>> GetOrdersInProductionAsync()
        {
            var productions = await _repo.GetProductionsInProgressAsync();

            return productions.Select(p => new ProductionOrderDto
            {
                order_id = p.order!.order_id,
                customer_name = p.order!.customer?.contact_name,
                quantity = p.order!.order_items.Sum(i => i.quantity),
                delivery_date = p.order!.delivery_date,
                production_status = p.status
            }).ToList();
        }

        public async Task<NearestDeliveryResponse> GetNearestDeliveryAsync()
        {
            var nearestDate = await _repo.GetNearestDeliveryDateAsync();

            var days = nearestDate == null
                ? 0
                : Math.Max(1, (nearestDate.Value.Date - DateTime.UtcNow.Date).Days);

            return new NearestDeliveryResponse
            {
                nearest_delivery_date = nearestDate,
                days_until_free = days
            };
        }
    }
}
