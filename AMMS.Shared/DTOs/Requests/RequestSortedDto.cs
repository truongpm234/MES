using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMMS.Shared.DTOs.Requests
{
    public record RequestSortedDto(
    int OrderRequestId,
    string CustomerName,
    string CustomerPhone,
    string? CustomerEmail,
    DateTime? DeliveryDate,
    string ProductName,
    int Quantity,
    string? ProcessStatus,
    DateTime? OrderRequestDate
);
}
