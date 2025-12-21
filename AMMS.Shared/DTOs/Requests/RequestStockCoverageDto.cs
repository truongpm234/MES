using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMMS.Shared.DTOs.Requests
{
    public record RequestStockCoverageDto(
        int OrderRequestId,
        string CustomerName,
        string CustomerPhone,
        string? CustomerEmail,
        DateTime? DeliveryDate,
        string ProductName,
        int Quantity,
        decimal StockQty,
        decimal CoverageRatio,   // stock / qty
        string? ProcessStatus,
        DateTime? OrderRequestDate
    );
}
