using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMMS.Shared.DTOs.Requests
{
    public class RequestPagedDto
    {
        public int order_request_id { get; set; }
        public string customer_name { get; set; } = "";
        public string customer_phone { get; set; } = "";
        public string? customer_email { get; set; }
        public DateTime? delivery_date { get; set; }
        public string? product_name { get; set; }
        public int? quantity { get; set; }
        public string? process_status { get; set; }
        public DateTime? order_request_date { get; set; }
        public decimal? final_cost { get; set; }
    }
}
