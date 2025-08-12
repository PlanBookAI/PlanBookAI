using Microsoft.EntityFrameworkCore;
using UserService.Models;
using UserService.Repositories;

namespace UserService.Data
{
    // Lớp UserDbContext kế thừa từ DbContext, dùng để tương tác với cơ sở dữ liệu
    public class UserDbContext : DbContext
    {
        // Constructor để nhận các tùy chọn DbContext (ví dụ: chuỗi kết nối)
        public UserDbContext(DbContextOptions<UserDbContext> options)
            : base(options)
        {
        }

        // DbSets để ánh xạ các class thành các bảng trong cơ sở dữ liệu
        public DbSet<NguoiDung> NguoiDungs { get; set; }
        public DbSet<HoSoNguoiDung> HoSoNguoiDungs { get; set; }

        // Cấu hình mô hình entity và mối quan hệ giữa các bảng
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Cấu hình mối quan hệ 1-1 giữa NguoiDung và HoSoNguoiDung
            // Mỗi NguoiDung có một HoSoNguoiDung và ngược lại
            modelBuilder.Entity<HoSoNguoiDung>()
                .HasOne(h => h.NguoiDung)
                .WithOne(n => n.HoSoNguoiDung)
                .HasForeignKey<HoSoNguoiDung>(h => h.NguoiDungId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
