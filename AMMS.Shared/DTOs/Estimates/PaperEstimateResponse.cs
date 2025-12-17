using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMMS.Shared.DTOs.Estimates
{
    public class PaperEstimateResponse
    {
        public int quantity { get; set; }
        public int paper_needed { get; set; }

        // trả thêm để FE show note (optional nhưng tiện)
        public int items_per_sheet { get; set; }
        public decimal wastage_percent { get; set; }
    }
}
