using AuthService.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Data
{
    /// <summary>
    /// Lớp DbContext quản lý việc truy cập cơ sở dữ liệu cho AuthService.
    /// Kết nối với schema 'users' trong PostgreSQL database.
    /// </summary>
    public class AuthDbContext : DbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
        {
        }

        // DbSet ánh xạ các lớp entity với các bảng trong database
        public DbSet<NguoiDung> NguoiDungs { get; set; }
        public DbSet<VaiTro> VaiTros { get; set; }
        public DbSet<PhienDangNhap> PhienDangNhaps { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình cho Entity NguoiDung
            modelBuilder.Entity<NguoiDung>(entity =>
            {
                // Chỉ mục duy nhất cho Email
                entity.HasIndex(nd => nd.Email)
                    .IsUnique()
                    .HasDatabaseName("idx_users_email");

                // Chỉ mục cho VaiTroId
                entity.HasIndex(nd => nd.VaiTroId)
                    .HasDatabaseName("idx_users_role");

                // Relationship với VaiTro
                entity.HasOne(nd => nd.VaiTro)
                    .WithMany(vt => vt.DanhSachNguoiDung)
                    .HasForeignKey(nd => nd.VaiTroId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Cấu hình cho Entity VaiTro
            modelBuilder.Entity<VaiTro>(entity =>
            {
                // Chỉ mục duy nhất cho Ten (name)
                entity.HasIndex(vt => vt.Ten)
                    .IsUnique()
                    .HasDatabaseName("idx_roles_name");
            });

            // Cấu hình cho Entity PhienDangNhap
            modelBuilder.Entity<PhienDangNhap>(entity =>
            {
                // Chỉ mục cho user_id
                entity.HasIndex(pdn => pdn.NguoiDungId)
                    .HasDatabaseName("idx_sessions_user");

                // Chỉ mục cho token
                entity.HasIndex(pdn => pdn.Token)
                    .HasDatabaseName("idx_sessions_token");

                // Relationship với NguoiDung
                entity.HasOne(pdn => pdn.NguoiDung)
                    .WithMany()
                    .HasForeignKey(pdn => pdn.NguoiDungId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Seed data cho bảng VaiTro (match với init-db.sql)
            modelBuilder.Entity<VaiTro>().HasData(
                new VaiTro 
                { 
                    Id = 1, 
                    Ten = "ADMIN", 
                    MoTa = "Quản trị viên hệ thống",
                    LaHoatDong = true,
                    TaoLuc = DateTime.UtcNow,
                    CapNhatLuc = DateTime.UtcNow
                },
                new VaiTro 
                { 
                    Id = 2, 
                    Ten = "MANAGER", 
                    MoTa = "Quản lý nội dung và người dùng",
                    LaHoatDong = true,
                    TaoLuc = DateTime.UtcNow,
                    CapNhatLuc = DateTime.UtcNow
                },
                new VaiTro 
                { 
                    Id = 3, 
                    Ten = "STAFF", 
                    MoTa = "Nhân viên tạo nội dung mẫu",
                    LaHoatDong = true,
                    TaoLuc = DateTime.UtcNow,
                    CapNhatLuc = DateTime.UtcNow
                },
                new VaiTro 
                { 
                    Id = 4, 
                    Ten = "TEACHER", 
                    MoTa = "Giáo viên sử dụng hệ thống",
                    LaHoatDong = true,
                    TaoLuc = DateTime.UtcNow,
                    CapNhatLuc = DateTime.UtcNow
                }
            );
        }
    }
}
