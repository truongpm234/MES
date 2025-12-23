using AMMS.Application.Interfaces;
using AMMS.Infrastructure.Interfaces;
using AMMS.Shared.DTOs.Boms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMMS.Application.Services
{
    public class MaterialPerUnitService : IMaterialPerUnitService
    {
        private readonly ICostEstimateRepository _estimateRepo;
        private readonly IRequestRepository _reqRepo;

        public MaterialPerUnitService(ICostEstimateRepository estimateRepo, IRequestRepository reqRepo)
        {
            _estimateRepo = estimateRepo;
            _reqRepo = reqRepo;
        }

        public async Task<MaterialPerUnitDto> GetMaterialPerUnitAsync(int orderRequestId)
        {
            var req = await _reqRepo.GetByIdAsync(orderRequestId)
                ?? throw new Exception("Order request not found");

            var est = await _estimateRepo.GetByOrderRequestIdAsync(orderRequestId)
                ?? throw new Exception("Estimate not found");

            var qty = req.quantity ?? 0;
            if (qty <= 0) throw new Exception("Order quantity invalid");

            decimal Div(decimal x) => Math.Round(x / qty, 6);

            return new MaterialPerUnitDto
            {
                order_request_id = orderRequestId,
                quantity = qty,
                paper_sheets_per_product = Div(est.paper_sheets_used),
                ink_kg_per_product = Div(est.ink_weight_kg),
                coating_glue_kg_per_product = Div(est.coating_glue_weight_kg),
                mounting_glue_kg_per_product = Div(est.mounting_glue_weight_kg),
                lamination_kg_per_product = Div(est.lamination_weight_kg),
            };
        }
    }
}
