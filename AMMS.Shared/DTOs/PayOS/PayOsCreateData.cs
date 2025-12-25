using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMMS.Shared.DTOs.PayOS
{
    public sealed class PayOsCreateData
    {
        public string? checkoutUrl { get; set; }
        public string? paymentLinkId { get; set; }
    }
}
