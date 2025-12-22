using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMMS.Shared.DTOs.Purchases
{
    public class CreatePurchaseRequestDto
    {
        public int? SupplierId { get; set; }     // optional: có thể null nếu chưa chọn NCC
        public DateTime? EtaDate { get; set; }   // optional
        public List<CreatePurchaseRequestItemDto> Items { get; set; } = new();
    }
}
