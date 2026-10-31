using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KhoLinhKien.Data.Migrations
{
    /// <inheritdoc />
    public partial class CapNhatPhieuNhap : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DonGiaNhap",
                table: "ChiTietPhieuNhap",
                newName: "DonGia");

            migrationBuilder.AddColumn<string>(
                name: "GhiChu",
                table: "PhieuNhap",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NguoiLap",
                table: "PhieuNhap",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GhiChu",
                table: "PhieuNhap");

            migrationBuilder.DropColumn(
                name: "NguoiLap",
                table: "PhieuNhap");

            migrationBuilder.RenameColumn(
                name: "DonGia",
                table: "ChiTietPhieuNhap",
                newName: "DonGiaNhap");
        }
    }
}
