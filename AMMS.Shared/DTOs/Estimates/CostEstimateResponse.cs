using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMMS.Shared.DTOs.Estimates
{
    public class CostEstimateResponse
    {
        public decimal base_cost { get; set; }
        public bool is_rush { get; set; }
        public decimal rush_percent { get; set; }
        public decimal rush_amount { get; set; }
        public decimal system_total_cost { get; set; }
        public DateTime estimated_finish_date { get; set; }
    }
}
