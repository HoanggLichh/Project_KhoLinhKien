namespace KhoLinhKien.Models;

public class LinhKien
{
    public int Id { get; set; }
    public string MaLinhKien { get; set; } = string.Empty;   // Ví dụ: LCD-IP11
    public string TenLinhKien { get; set; } = string.Empty;

    public int LoaiLinhKienId { get; set; }        // Khóa ngoại tới loại linh kiện
    public LoaiLinhKien? LoaiLinhKien { get; set; }

    public int? ViTriKhoId { get; set; }           // Có thể chưa gán vị trí
    public ViTriKho? ViTriKho { get; set; }

    public string? DongMayTuongThich { get; set; } // Ví dụ: iPhone 11
    public decimal DonGia { get; set; }
    public int SoLuongTon { get; set; }            // Chỉ đổi qua phiếu nhập / đơn sửa
    public int TonToiThieu { get; set; }           // Dưới mức này thì cảnh báo
}
