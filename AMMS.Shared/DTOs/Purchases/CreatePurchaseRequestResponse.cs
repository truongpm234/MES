using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMMS.Shared.DTOs.Purchases
{
    public record CreatePurchaseRequestResponse(
        int PurchaseId,
        string? Code,
        string Status,
        DateTime? CreatedAt
    );
}
