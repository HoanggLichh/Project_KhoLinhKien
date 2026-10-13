using KhoLinhKien.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace KhoLinhKien.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // Mỗi DbSet là một bảng trong database
    public DbSet<LoaiLinhKien> LoaiLinhKien { get; set; }
    public DbSet<NhaCungCap> NhaCungCap { get; set; }
    public DbSet<ViTriKho> ViTriKho { get; set; }
    public DbSet<LinhKien> LinhKien { get; set; }
    public DbSet<PhieuNhap> PhieuNhap { get; set; }
    public DbSet<ChiTietPhieuNhap> ChiTietPhieuNhap { get; set; }
    public DbSet<DonSuaChua> DonSuaChua { get; set; }
    public DbSet<ChiTietDonSuaChua> ChiTietDonSuaChua { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);   // Bắt buộc giữ dòng này cho phần Identity

        // Mã linh kiện không được trùng
        builder.Entity<LinhKien>().HasIndex(x => x.MaLinhKien).IsUnique();

        // Tiền lưu 18 chữ số, 2 số lẻ
        builder.Entity<LinhKien>().Property(x => x.DonGia).HasPrecision(18, 2);
        builder.Entity<ChiTietPhieuNhap>().Property(x => x.DonGiaNhap).HasPrecision(18, 2);
    }
}
