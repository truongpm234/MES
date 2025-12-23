using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMMS.Shared.DTOs.Productions
{
    public class ScanTaskResult
    {
        public int task_id { get; set; }
        public int? prod_id { get; set; }
        public string message { get; set; } = "OK";
    }
}
