using Microsoft.EntityFrameworkCore;
using UserService.Models;
using UserService.Models.Entities;

namespace UserService.Data
{
    /// <summary>
    /// Lớp UserDbContext quản lý việc truy cập cơ sở dữ liệu cho UserService.
    /// Kết nối với schema 'users' trong PostgreSQL database.
    /// </summary>
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
        {
        }

        // DbSet ánh xạ các lớp entity với các bảng trong database
        public DbSet<HoSoNguoiDung> HoSoNguoiDungs { get; set; }
        public DbSet<LichSuHoatDong> LichSuHoatDongs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình cho Entity HoSoNguoiDung
            modelBuilder.Entity<HoSoNguoiDung>(entity =>
            {
                // Primary key
                entity.HasKey(h => h.UserId);
            });

            // Cấu hình cho Entity LichSuHoatDong  
            modelBuilder.Entity<LichSuHoatDong>(entity =>
            {
                // Primary key
                entity.HasKey(l => l.Id);
            });
        }
    }
}
