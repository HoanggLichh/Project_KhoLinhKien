using System.ComponentModel.DataAnnotations;

namespace KhoLinhKien.Models;

public class PhieuNhapCreateVM
{
    [Required(ErrorMessage = "Vui lòng chọn nhà cung cấp")]
    [Display(Name = "Nhà cung cấp")]
    public int? NhaCungCapId { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Ngày nhập")]
    public DateTime NgayNhap { get; set; } = DateTime.Today;

    [StringLength(250, ErrorMessage = "Tối đa 250 ký tự")]
    [Display(Name = "Ghi chú")]
    public string? GhiChu { get; set; }

    public List<DongNhapVM> Dongs { get; set; } = new();
}

public class DongNhapVM
{
    public int LinhKienId { get; set; }
    public int SoLuong { get; set; }
    public decimal DonGia { get; set; }
}

// Dùng để đổ danh sách linh kiện vào ô chọn
public class LinhKienChonVM
{
    public int Id { get; set; }
    public string Ten { get; set; } = string.Empty;
    public decimal DonGia { get; set; }
    public int Ton { get; set; }
}