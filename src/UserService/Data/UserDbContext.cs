using Microsoft.EntityFrameworkCore;
using UserService.Models.Entities;

namespace UserService.Data;

public class UserDbContext : DbContext
{
	public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
	{
	}

	// DbSets
	public DbSet<NguoiDung> NguoiDung { get; set; }
	public DbSet<VaiTro> VaiTro { get; set; }
	public DbSet<HoSoNguoiDung> HoSoNguoiDung { get; set; }
    public DbSet<OtpCode> OtpCodes { get; set; }
    public DbSet<PasswordHistory> PasswordHistory { get; set; }
    // Không dùng DbSet<LichSuDangNhap> để EF Core không can thiệp
    // public DbSet<LichSuDangNhap> LichSuDangNhap { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		// Không cần global DateTime conversion - database sử dụng timestamp without time zone

		// Cấu hình NguoiDung
		modelBuilder.Entity<NguoiDung>(entity =>
		{
			entity.ToTable("users", "auth");
			entity.HasKey(e => e.Id);
			entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
			entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(255);
			entity.Property(e => e.RoleId).IsRequired();
			entity.Property(e => e.IsActive).HasDefaultValue(true);
			entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
			entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
			
			// Relationships
			entity.HasOne(e => e.VaiTro)
				  .WithMany(e => e.NguoiDung)
				  .HasForeignKey(e => e.RoleId)
				  .OnDelete(DeleteBehavior.Restrict);

			// Profile relationship
			entity.HasOne(e => e.HoSoNguoiDung)
				  .WithOne(e => e.NguoiDung)
				  .HasForeignKey<HoSoNguoiDung>(e => e.UserId)
				  .OnDelete(DeleteBehavior.Cascade);

			// Không cấu hình relationship LichSuDangNhap để tránh EF Core tự động join
			// entity.HasMany(e => e.LichSuDangNhap)
			//       .WithOne(e => e.NguoiDung)
			//       .HasForeignKey(e => e.UserId)
			//       .OnDelete(DeleteBehavior.Cascade);
		});

		// Cấu hình VaiTro
		modelBuilder.Entity<VaiTro>(entity =>
		{
			entity.ToTable("roles", "auth");
			entity.HasKey(e => e.Id);
			entity.Property(e => e.Ten).IsRequired().HasMaxLength(50);
			entity.Property(e => e.MoTa);
			entity.Property(e => e.IsActive).HasDefaultValue(true);
			entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
			entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
		});

		// Cấu hình HoSoNguoiDung
		modelBuilder.Entity<HoSoNguoiDung>(entity =>
		{
			entity.ToTable("user_profiles", "users");
			entity.HasKey(e => e.Id);
			entity.Property(e => e.UserId).IsRequired();
			entity.Property(e => e.HoTen).IsRequired().HasMaxLength(255);
			entity.Property(e => e.SoDienThoai).HasMaxLength(20);
			entity.Property(e => e.DiaChi);
			entity.Property(e => e.MoTaBanThan);
			entity.Property(e => e.AnhDaiDienUrl).HasMaxLength(500);
			entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
			entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
		});

		// Không cấu hình LichSuDangNhap để EF Core không can thiệp
		// modelBuilder.Entity<LichSuDangNhap>(entity =>
		// {
		// 	entity.ToTable("sessions", "auth");
		// 	entity.HasKey(e => e.Id);
		// 	entity.Property(e => e.UserId).IsRequired();
		// 	entity.Property(e => e.Token).IsRequired().HasMaxLength(500);
		// 	entity.Property(e => e.ExpiresAt);
		// 	entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
		// });
	}
}
