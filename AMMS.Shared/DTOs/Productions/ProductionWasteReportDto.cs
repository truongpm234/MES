using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMMS.Shared.DTOs.Productions
{
    public sealed class ProductionWasteReportDto
    {
        public int prod_id { get; set; }
        public decimal total_good { get; set; }
        public decimal total_bad { get; set; }
        public decimal total_waste_percent { get; set; }

        public List<StageWasteDto> stages { get; set; } = new();
    }
}

