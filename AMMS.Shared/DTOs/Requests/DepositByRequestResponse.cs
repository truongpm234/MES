using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMMS.Shared.DTOs.Requests
{
    public class DepositByRequestResponse
    {
        public int order_request_id { get; set; }
        public decimal deposit_amount { get; set; }
    }
}
