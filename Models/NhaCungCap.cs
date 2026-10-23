using System.ComponentModel.DataAnnotations;

namespace KhoLinhKien.Models;

public class NhaCungCap
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập tên nhà cung cấp")]
    [StringLength(150, ErrorMessage = "Tối đa 150 ký tự")]
    [Display(Name = "Tên nhà cung cấp")]
    public string TenNhaCungCap { get; set; } = string.Empty;

    [StringLength(20, ErrorMessage = "Tối đa 20 ký tự")]
    [Display(Name = "Số điện thoại")]
    public string? SoDienThoai { get; set; }

    [StringLength(250, ErrorMessage = "Tối đa 250 ký tự")]
    [Display(Name = "Địa chỉ")]
    public string? DiaChi { get; set; }
}