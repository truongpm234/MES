using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMMS.Shared.DTOs.Estimates
{
    public class CostEstimateResponse
    {
        public decimal paper_cost { get; set; }
        public decimal ink_cost { get; set; }
        public decimal coating_glue_cost { get; set; }
        public decimal mounting_glue_cost { get; set; }
        public decimal lamination_cost { get; set; }
        public decimal material_cost { get; set; }
        public decimal overhead_cost { get; set; }
        public decimal base_cost { get; set; }
        public bool is_rush { get; set; }
        public decimal rush_percent { get; set; }
        public decimal rush_amount { get; set; }
        public decimal system_total_cost { get; set; }
        public DateTime estimated_finish_date { get; set; }
    }
}
