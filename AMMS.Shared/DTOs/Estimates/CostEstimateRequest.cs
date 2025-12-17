using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMMS.Shared.DTOs.Estimates
{
    public class CostEstimateRequest
    {
        public int order_request_id { get; set; }
        public PaperEstimateResponse paper { get; set; } = null!;
        public DateTime desired_delivery_date { get; set; }
    }

}

