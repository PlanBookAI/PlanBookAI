using AuthService.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;

namespace AuthService.Data
{
    /// <summary>
    /// Lớp DbContext quản lý việc truy cập cơ sở dữ liệu cho AuthService.
    /// </summary>
    public class AuthDbContext : DbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
        {
        }

        // DbSet ánh xạ các lớp entity với các bảng trong database.
        public DbSet<NguoiDung> NguoiDungs { get; set; }
        public DbSet<VaiTro> VaiTros { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Cấu hình chỉ mục duy nhất cho TenDangNhap.
            modelBuilder.Entity<NguoiDung>()
                .HasIndex(nd => nd.TenDangNhap)
                .IsUnique();

            // Thêm dữ liệu ban đầu cho bảng VaiTro.
            modelBuilder.Entity<VaiTro>().HasData(
                new VaiTro { Id = Guid.NewGuid(), TenVaiTro = "Admin" },
                new VaiTro { Id = Guid.NewGuid(), TenVaiTro = "User" }
            );
        }
    }
}