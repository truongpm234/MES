namespace AMMS.Shared.DTOs.Estimates
{
    public class PaperEstimateResponse
    {
        public string paper_code { get; set; } = null!;
        public int sheet_width_mm { get; set; }  // kich thuoc nvl
        public int sheet_height_mm { get; set; }
        public int sheet_length_mm { get; set; }
        public int print_width_mm { get; set; }   // kich thuoc cua don hang
        public int print_height_mm { get; set; }
        public int print_length_mm { get; set; }
        public int n_up { get; set; }   //so luong sp in duoc tren 1 sheet
        public int quantity { get; set; }
        public int sheets_base { get; set; }  //so luong thuc te chua tinh hao hut
        public int sheets_with_waste { get; set; }  //so luong bao da tinh hao hut
        public decimal waste_percent { get; set; }  // % hao hut
    }
}
