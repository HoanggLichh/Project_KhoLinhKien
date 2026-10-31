using System.ComponentModel.DataAnnotations;

namespace KhoLinhKien.Models;

public static class TrangThaiDon
{
    public const string NhanMay = "NhanMay";
    public const string DangSua = "DangSua";
    public const string HoanTat = "HoanTat";

    public static string Ten(string? t) => t switch
    {
        NhanMay => "Nhận máy",
        DangSua => "Đang sửa",
        HoanTat => "Hoàn tất",
        _ => "Không rõ"
    };

    // Dùng cho class màu của nhãn trạng thái (pill)
    public static string Mau(string? t) => t switch
    {
        HoanTat => "done",
        DangSua => "doing",
        _ => "wait"
    };
}

public class DonSuaChua
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Nhập tên khách hàng")]
    [StringLength(100)]
    [Display(Name = "Tên khách hàng")]
    public string TenKhach { get; set; } = string.Empty;

    [Required(ErrorMessage = "Nhập số điện thoại")]
    [RegularExpression(@"^0\d{9,10}$", ErrorMessage = "Số điện thoại phải bắt đầu bằng 0 và có 10-11 số")]
    [Display(Name = "Số điện thoại")]
    public string SoDienThoai { get; set; } = string.Empty;

    [Required(ErrorMessage = "Nhập tên máy")]
    [StringLength(100)]
    [Display(Name = "Máy cần sửa")]
    public string TenMay { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mô tả lỗi của máy")]
    [StringLength(500)]
    [Display(Name = "Mô tả lỗi")]
    public string MoTaLoi { get; set; } = string.Empty;

    [DataType(DataType.Date)]
    [Display(Name = "Ngày nhận")]
    public DateTime NgayNhan { get; set; } = DateTime.Today;

    [StringLength(20)]
    [Display(Name = "Trạng thái")]
    public string TrangThai { get; set; } = TrangThaiDon.NhanMay;

    [Display(Name = "Tiền công")]
    [Range(0, 1000000000, ErrorMessage = "Tiền công không hợp lệ")]
    public decimal CongTho { get; set; }

    [StringLength(256)]
    public string? NguoiLap { get; set; }

    public List<ChiTietDonSuaChua> ChiTiets { get; set; } = new();
}