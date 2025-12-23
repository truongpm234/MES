using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMMS.Shared.DTOs.Enums
{
    public static class RoutingDefinitions
    {
        public static readonly Dictionary<ProcessType, string> ProcessDisplay = new()
    {
        { ProcessType.RALO, "Ralo" },
        { ProcessType.CAT, "Cắt" },
        { ProcessType.IN, "In" },
        { ProcessType.PHU, "Phủ" },
        { ProcessType.BOI, "Bồi" },
        { ProcessType.BE, "Bế" },
        { ProcessType.DUT, "Dứt" },
        { ProcessType.DAN, "Dán" },
        { ProcessType.DOT, "Đóng gói" },
        { ProcessType.CAN_MANG, "Cán màng" },
    };

        public static readonly Dictionary<ProductTypeCodeGeneral, ProcessType[]> Routing = new()
    {
        { ProductTypeCodeGeneral.HOP_MAU, new[]{ ProcessType.RALO, ProcessType.CAT, ProcessType.IN, ProcessType.PHU, ProcessType.BOI, ProcessType.BE, ProcessType.DUT, ProcessType.DAN, ProcessType.DOT } },
        { ProductTypeCodeGeneral.KHAY, new[]{ ProcessType.RALO, ProcessType.CAT, ProcessType.IN, ProcessType.PHU, ProcessType.BOI, ProcessType.BE, ProcessType.DUT, ProcessType.DAN, ProcessType.DOT } },
        { ProductTypeCodeGeneral.VO_HOP_GACH, new[]{ ProcessType.RALO, ProcessType.CAT, ProcessType.IN, ProcessType.BOI, ProcessType.BE, ProcessType.DUT, ProcessType.DOT } },
        { ProductTypeCodeGeneral.THE_MAU, new[]{ ProcessType.RALO, ProcessType.CAT, ProcessType.IN, ProcessType.PHU, ProcessType.CAT, ProcessType.DOT } },
        { ProductTypeCodeGeneral.KHAC, new[]{ ProcessType.RALO, ProcessType.CAT, ProcessType.IN, ProcessType.PHU, ProcessType.BOI, ProcessType.BE, ProcessType.DUT, ProcessType.DAN, ProcessType.DOT } },
    };
    }

}
