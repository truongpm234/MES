using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMMS.Shared.DTOs.Productions
{
    public class NearestDeliveryResponse
    {
        public DateTime? nearest_delivery_date { get; set; }
        public int days_until_free { get; set; }
    }
}
