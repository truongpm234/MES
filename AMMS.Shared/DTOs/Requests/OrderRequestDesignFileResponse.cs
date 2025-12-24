using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMMS.Shared.DTOs.Requests
{
    public class OrderRequestDesignFileResponse
    {
        public int order_request_id { get; set; }
        public string? design_file_path { get; set; }
    }
}
