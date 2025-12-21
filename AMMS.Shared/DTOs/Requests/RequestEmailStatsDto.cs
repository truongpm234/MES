using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMMS.Shared.DTOs.Requests
{
    public record RequestEmailStatsDto(
       string CustomerEmail,
       int AcceptedCount
   );
}
