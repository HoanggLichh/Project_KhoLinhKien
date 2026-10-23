using System.ComponentModel.DataAnnotations;

namespace KhoLinhKien.Models;

public class LinhKien
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập mã linh kiện")]
    [StringLength(50, ErrorMessage = "Mã tối đa 50 ký tự")]
    [Display(Name = "Mã linh kiện")]
    public string MaLinhKien { get; set; } = string.Empty;   // Ví dụ: LCD-IP11

    [Required(ErrorMessage = "Vui lòng nhập tên linh kiện")]
    [StringLength(200, ErrorMessage = "Tên tối đa 200 ký tự")]
    [Display(Name = "Tên linh kiện")]
    public string TenLinhKien { get; set; } = string.Empty;

    [Display(Name = "Loại linh kiện")]
    public int LoaiLinhKienId { get; set; }
    public LoaiLinhKien? LoaiLinhKien { get; set; }

    [Display(Name = "Vị trí kho")]
    public int? ViTriKhoId { get; set; }
    public ViTriKho? ViTriKho { get; set; }

    [StringLength(200, ErrorMessage = "Tối đa 200 ký tự")]
    [Display(Name = "Dòng máy tương thích")]
    public string? DongMayTuongThich { get; set; }

    [Range(0, 1000000000, ErrorMessage = "Đơn giá phải từ 0 trở lên")]
    [Display(Name = "Đơn giá")]
    public decimal DonGia { get; set; }

    [Display(Name = "Số lượng tồn")]
    public int SoLuongTon { get; set; }   // Chỉ đổi qua phiếu nhập / đơn sửa

    [Range(0, 100000, ErrorMessage = "Tồn tối thiểu phải từ 0 trở lên")]
    [Display(Name = "Tồn tối thiểu")]
    public int TonToiThieu { get; set; }
}