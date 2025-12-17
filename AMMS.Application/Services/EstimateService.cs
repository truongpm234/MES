using AMMS.Application.Interfaces;
using AMMS.Shared.DTOs.Estimates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMMS.Application.Services
{
    public class EstimateService : IEstimateService
    {
        
        private const int ItemsPerSheet = 4;
        private const decimal WastagePercent = 5m;

        public Task<PaperEstimateResponse> EstimatePaperAsync(PaperEstimateRequest req)
        {
            if (req.quantity <= 0)
                throw new ArgumentException("quantity must be > 0");

            var factor = 1m + (WastagePercent / 100m);
            var raw = (req.quantity / (decimal)ItemsPerSheet) * factor;
            var paperNeeded = (int)Math.Ceiling(raw);

            return Task.FromResult(new PaperEstimateResponse
            {
                quantity = req.quantity,
                paper_needed = paperNeeded,
                items_per_sheet = ItemsPerSheet,
                wastage_percent = WastagePercent
            });
        }
    }
}
