using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMMS.Shared.DTOs.Productions
{
    public class ProductionOrderDto
    {
        public int order_id { get; set; }
        public string? customer_name { get; set; }
        public int quantity { get; set; }
        public DateTime? delivery_date { get; set; }
        public string? production_status { get; set; }
    }
}
