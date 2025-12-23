using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMMS.Shared.DTOs.Orders
{
    public class OrderListDto
    {
        public int Order_id { get; set; }
        public string Code { get; set; } = "";
        public DateTime? Order_date { get; set; }
        public DateTime? Delivery_date { get; set; }
        public string? Status { get; set; }
        public string? Payment_status { get; set; }
        public int? Quote_id { get; set; }
        public decimal? Total_amount { get; set; }
    }
}
