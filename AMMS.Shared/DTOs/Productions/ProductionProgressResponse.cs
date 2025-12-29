using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMMS.Shared.DTOs.Productions
{
    public class ProductionProgressResponse
    {
        public int prod_id { get; set; }
        public int total_steps { get; set; }
        public int finished_steps { get; set; }
        public decimal progress_percent { get; set; }
    }

}
