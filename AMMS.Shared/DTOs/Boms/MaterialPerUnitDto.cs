using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMMS.Shared.DTOs.Boms
{
    public class MaterialPerUnitDto
    {
        public int order_request_id { get; set; }
        public int quantity { get; set; }

        public decimal paper_sheets_per_product { get; set; }
        public decimal ink_kg_per_product { get; set; }
        public decimal coating_glue_kg_per_product { get; set; }
        public decimal mounting_glue_kg_per_product { get; set; }
        public decimal lamination_kg_per_product { get; set; }
    }
}
