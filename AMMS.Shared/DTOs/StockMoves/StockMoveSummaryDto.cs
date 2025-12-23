using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMMS.Shared.DTOs.StockMoves
{
    public record StockMoveSummaryDto(
         int MaterialId,
         string MaterialCode,
         string MaterialName,
         string Unit,
         decimal CurrentStock,    // materials.stock_qty
         decimal TotalIn,         // tổng qty type = IN
         decimal TotalOut,        // tổng qty type = OUT
         decimal TotalReturn,     // tổng qty type = RETURN
         decimal CalculatedStock  // TotalIn + TotalReturn - TotalOut
     );
}
