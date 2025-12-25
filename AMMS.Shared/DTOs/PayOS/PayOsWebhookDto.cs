using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMMS.Shared.DTOs.PayOS
{
    public sealed class PayOsWebhookDto
    {
        public int orderCode { get; set; }
        public string? status { get; set; } 
        public int? amount { get; set; }  
        public string? paymentLinkId { get; set; }
        public string? transactionId { get; set; }
        public object? raw { get; set; }  
    }

}
