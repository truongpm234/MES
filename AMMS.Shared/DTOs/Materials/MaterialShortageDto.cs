using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMMS.Shared.DTOs.Materials
{
    public class MaterialShortageDto
    {
        public int MaterialId { get; set; }
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Unit { get; set; } = null!;

        public decimal StockQty { get; set; }      // tồn kho hiện tại
        public decimal RequiredQty { get; set; }   // tổng nhu cầu
        public decimal ShortageQty { get; set; }   // thiếu = Required - Stock
        public decimal NeedToBuyQty { get; set; }  // số mặc định đề xuất mua (ban đầu = ShortageQty)
    }
}
