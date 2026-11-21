namespace KhoLinhKien.Models
{
    public class LichSuDong
    {
        public DateTime Ngay { get; set; }
        public string Loai { get; set; } = "";      // Nhập / Xuất
        public string ChungTu { get; set; } = "";   // PN-0003 / DSC-0005
        public int SoLuong { get; set; }
    }
}