using Microsoft.EntityFrameworkCore;
using AuthService.Models.Entities;
using BCrypt.Net;

namespace AuthService.Data;

public class AuthDbContext : DbContext
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
    {
    }

    public DbSet<VaiTro> VaiTros { get; set; }
    public DbSet<NguoiDung> NguoiDungs { get; set; }
    public DbSet<PhienDangNhap> PhienDangNhaps { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Cấu hình VaiTro
        modelBuilder.Entity<VaiTro>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Ten).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => e.Ten).IsUnique();
        });

        // Cấu hình NguoiDung
        modelBuilder.Entity<NguoiDung>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.MatKhauMaHoa).IsRequired().HasMaxLength(255);
            
            entity.HasOne(e => e.VaiTro)
                  .WithMany(e => e.DanhSachNguoiDung)
                  .HasForeignKey(e => e.VaiTroId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Cấu hình PhienDangNhap
        modelBuilder.Entity<PhienDangNhap>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Token).IsRequired().HasMaxLength(500);
            entity.HasIndex(e => e.Token).IsUnique();
            
            entity.HasOne(e => e.NguoiDung)
                  .WithMany(e => e.DanhSachPhienDangNhap)
                  .HasForeignKey(e => e.NguoiDungId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Seed data cho VaiTro
        modelBuilder.Entity<VaiTro>().HasData(
            new VaiTro { Id = 1, Ten = "ADMIN", MoTa = "Quản trị viên hệ thống", HoatDong = true },
            new VaiTro { Id = 2, Ten = "MANAGER", MoTa = "Quản lý nội dung và người dùng", HoatDong = true },
            new VaiTro { Id = 3, Ten = "STAFF", MoTa = "Nhân viên tạo nội dung mẫu", HoatDong = true },
            new VaiTro { Id = 4, Ten = "TEACHER", MoTa = "Giáo viên sử dụng hệ thống", HoatDong = true }
        );

        // Seed data cho User Admin (mật khẩu: admin123)
        var adminPasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123");
        modelBuilder.Entity<NguoiDung>().HasData(
            new NguoiDung 
            { 
                Id = Guid.Parse("550e8400-e29b-41d4-a716-446655440001"), 
                Email = "admin@planbookai.com", 
                MatKhauMaHoa = adminPasswordHash, 
                VaiTroId = 1, 
                HoatDong = true 
            }
        );
    }
}
