using AMMS.Shared.DTOs.Enums;

namespace AMMS.Shared.DTOs.Estimates
{
    public class PaperEstimateRequest
    {
        public int order_request_id { get; set; }

        public string paper_code { get; set; } = null!;

        public int quantity { get; set; }

        public int length_mm { get; set; }

        public int width_mm { get; set; }

        public int height_mm { get; set; }

        public int glue_tab_mm { get; set; } = 15;

        public int bleed_mm { get; set; } = 1;

        public bool is_one_side_box { get; set; } = false;

        public string product_type { get; set; } = "";

        public string? form_product { get; set; }

        public int number_of_plates { get; set; } = 0;

        public string production_processes { get; set; } = "IN";

        public string coating_type { get; set; } = "KEO_NUOC";

        public string? wave_type { get; set; }
    }
}
