using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMMS.Shared.DTOs.Productions
{
    public class ScanTaskRequest
    {
        public string token { get; set; } = null!;
        public string? scanner_id { get; set; }
        public int? operator_id { get; set; }
        public int? qty_good { get; set; }
        public int? qty_bad { get; set; }
    }
}
