using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMMS.Shared.DTOs.PayOS
{
    public sealed class PayOsCreateResponse
    {
        public string? code { get; set; }
        public string? desc { get; set; }
        public PayOsCreateData? data { get; set; }
    }
}
