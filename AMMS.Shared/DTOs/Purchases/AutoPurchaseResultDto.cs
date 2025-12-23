using AMMS.Shared.DTOs.Materials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMMS.Shared.DTOs.Purchases
{
    public class AutoPurchaseResultDto
    {
        public int PurchaseId { get; set; }
        public string PurchaseCode { get; set; } = null!;
        public List<MaterialShortageDto> Items { get; set; } = new();
    }
}
