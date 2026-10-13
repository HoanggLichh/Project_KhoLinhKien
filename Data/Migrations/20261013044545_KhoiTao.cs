using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KhoLinhKien.Data.Migrations
{
    /// <inheritdoc />
    public partial class KhoiTao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DonSuaChua",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenKhach = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TenMay = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MoTaLoi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayNhan = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DonSuaChua", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LoaiLinhKien",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenLoai = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoaiLinhKien", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NhaCungCap",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenNhaCungCap = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DiaChi = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhaCungCap", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ViTriKho",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenViTri = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ViTriKho", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PhieuNhap",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NgayNhap = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NhaCungCapId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhieuNhap", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PhieuNhap_NhaCungCap_NhaCungCapId",
                        column: x => x.NhaCungCapId,
                        principalTable: "NhaCungCap",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LinhKien",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaLinhKien = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TenLinhKien = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LoaiLinhKienId = table.Column<int>(type: "int", nullable: false),
                    ViTriKhoId = table.Column<int>(type: "int", nullable: true),
                    DongMayTuongThich = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DonGia = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    SoLuongTon = table.Column<int>(type: "int", nullable: false),
                    TonToiThieu = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LinhKien", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LinhKien_LoaiLinhKien_LoaiLinhKienId",
                        column: x => x.LoaiLinhKienId,
                        principalTable: "LoaiLinhKien",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LinhKien_ViTriKho_ViTriKhoId",
                        column: x => x.ViTriKhoId,
                        principalTable: "ViTriKho",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ChiTietDonSuaChua",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DonSuaChuaId = table.Column<int>(type: "int", nullable: false),
                    LinhKienId = table.Column<int>(type: "int", nullable: false),
                    SoLuong = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTietDonSuaChua", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChiTietDonSuaChua_DonSuaChua_DonSuaChuaId",
                        column: x => x.DonSuaChuaId,
                        principalTable: "DonSuaChua",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChiTietDonSuaChua_LinhKien_LinhKienId",
                        column: x => x.LinhKienId,
                        principalTable: "LinhKien",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChiTietPhieuNhap",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PhieuNhapId = table.Column<int>(type: "int", nullable: false),
                    LinhKienId = table.Column<int>(type: "int", nullable: false),
                    SoLuong = table.Column<int>(type: "int", nullable: false),
                    DonGiaNhap = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTietPhieuNhap", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChiTietPhieuNhap_LinhKien_LinhKienId",
                        column: x => x.LinhKienId,
                        principalTable: "LinhKien",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChiTietPhieuNhap_PhieuNhap_PhieuNhapId",
                        column: x => x.PhieuNhapId,
                        principalTable: "PhieuNhap",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietDonSuaChua_DonSuaChuaId",
                table: "ChiTietDonSuaChua",
                column: "DonSuaChuaId");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietDonSuaChua_LinhKienId",
                table: "ChiTietDonSuaChua",
                column: "LinhKienId");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietPhieuNhap_LinhKienId",
                table: "ChiTietPhieuNhap",
                column: "LinhKienId");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietPhieuNhap_PhieuNhapId",
                table: "ChiTietPhieuNhap",
                column: "PhieuNhapId");

            migrationBuilder.CreateIndex(
                name: "IX_LinhKien_LoaiLinhKienId",
                table: "LinhKien",
                column: "LoaiLinhKienId");

            migrationBuilder.CreateIndex(
                name: "IX_LinhKien_MaLinhKien",
                table: "LinhKien",
                column: "MaLinhKien",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LinhKien_ViTriKhoId",
                table: "LinhKien",
                column: "ViTriKhoId");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuNhap_NhaCungCapId",
                table: "PhieuNhap",
                column: "NhaCungCapId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChiTietDonSuaChua");

            migrationBuilder.DropTable(
                name: "ChiTietPhieuNhap");

            migrationBuilder.DropTable(
                name: "DonSuaChua");

            migrationBuilder.DropTable(
                name: "LinhKien");

            migrationBuilder.DropTable(
                name: "PhieuNhap");

            migrationBuilder.DropTable(
                name: "LoaiLinhKien");

            migrationBuilder.DropTable(
                name: "ViTriKho");

            migrationBuilder.DropTable(
                name: "NhaCungCap");
        }
    }
}
