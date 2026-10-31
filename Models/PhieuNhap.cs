using System.ComponentModel.DataAnnotations;

namespace KhoLinhKien.Models;

public class PhieuNhap
{
    public int Id { get; set; }

    [Display(Name = "Nhà cung cấp")]
    public int NhaCungCapId { get; set; }
    public NhaCungCap? NhaCungCap { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Ngày nhập")]
    public DateTime NgayNhap { get; set; } = DateTime.Today;

    [StringLength(250)]
    [Display(Name = "Ghi chú")]
    public string? GhiChu { get; set; }

    [StringLength(256)]
    [Display(Name = "Người lập")]
    public string? NguoiLap { get; set; }

    public List<ChiTietPhieuNhap> ChiTiets { get; set; } = new();
}