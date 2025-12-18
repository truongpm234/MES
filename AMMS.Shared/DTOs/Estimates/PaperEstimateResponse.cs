namespace AMMS.Shared.DTOs.Estimates
{
    public class PaperEstimateResponse
    {
        public string paper_code { get; set; } = null!;
        public int sheet_width_mm { get; set; }   //kich thuoc nvl
        public int sheet_height_mm { get; set; }
        public int sheet_length_mm { get; set; }
        public int print_width_mm { get; set; }   //kich thuoc sp in tren nvl
        public int print_height_mm { get; set; }
        public int print_length_mm { get; set; }
        public int n_up { get; set; }   //SL sp tren nvl
        public int quantity { get; set; }
        public int sheets_base { get; set; }  //so luong chua tinh hao hut

        //CHI TIẾT HAO HỤT
        public int waste_printing { get; set; }
        public int waste_die_cutting { get; set; }  //hao hut cat die
        public int waste_mounting { get; set; }   //hao hut be
        public int waste_coating { get; set; }   //hao hut phu
        public int waste_lamination { get; set; }   //hao hut can mang
        public int waste_gluing { get; set; }   //hao hut dan keo
        public int total_waste { get; set; } //tong hao hut
        public int sheets_with_waste { get; set; }   //sl nvl giay bao gom hao hut
        public decimal waste_percent { get; set; }   //phan tram hao hut
    }

}
