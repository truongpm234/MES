namespace AMMS.Shared.DTOs.Estimates
{
    public class PaperEstimateRequest
    {
        public string paper_code { get; set; } = null!;
        public int quantity { get; set; }

        // Kích thước sản phẩm (mm)
        public int length_mm { get; set; }
        public int width_mm { get; set; }
        public int height_mm { get; set; }

        // Tuỳ chỉnh
        public int allowance_mm { get; set; } = 5;  //chua gap/dan
        public int bleed_mm { get; set; } = 1;   //chua xen

        // Loại sản phẩm in
        public string product_type { get; set; } = "HOP_MAU_1LUOT_THUONG";

        // Số cao bản (cho hộp màu)
        public int number_of_plates { get; set; } = 0;

        // Công đoạn (danh sách cách nhau bởi dấu phẩy)
        // Ví dụ: "IN,BE,BOI,DAN,PHU,CAN_MANG"
        public string production_processes { get; set; } = "IN";

        // Loại phủ
        public string coating_type { get; set; } = "NONE"; // "KEO_NUOC", "KEO_DAU", "NONE"
    }
}
