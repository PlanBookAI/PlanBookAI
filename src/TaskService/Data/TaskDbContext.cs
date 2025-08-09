using Microsoft.EntityFrameworkCore;
using TaskService.Models.Entities;

namespace TaskService.Data
{
    // Lớp TaskDbContext kế thừa từ DbContext
    public class TaskDbContext : DbContext
    {
        // Constructor để nhận các tùy chọn DbContext
        public TaskDbContext(DbContextOptions<TaskDbContext> options)
            : base(options)
        {
        }

        // DbSets để ánh xạ các entities với các bảng trong cơ sở dữ liệu
        public DbSet<GiaoAn> GiaoAns { get; set; }
        public DbSet<NguoiDung> NguoiDungs { get; set; }
        public DbSet<HocSinh> HocSinhs { get; set; }
        public DbSet<DeThi> DeThis { get; set; }
        public DbSet<CauHoi> CauHois { get; set; }
        public DbSet<KetQua> KetQuas { get; set; }
        public DbSet<BaiLam> BaiLams { get; set; }

        // Phương thức OnModelCreating để cấu hình các mô hình entity
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Mối quan hệ 1-nhiều: HocSinh (1) -> BaiLam (N)
            modelBuilder.Entity<BaiLam>()
                .HasOne<HocSinh>()
                .WithMany(h => h.BaiLams)
                .HasForeignKey(b => b.hocSinhId);

            // Mối quan hệ 1-nhiều: DeThi (1) -> BaiLam (N)
            modelBuilder.Entity<BaiLam>()
                .HasOne<DeThi>()
                .WithMany(d => d.BaiLams)
                .HasForeignKey(b => b.deThiId);

            // Mối quan hệ 1-1: BaiLam (1) -> KetQua (1)
            modelBuilder.Entity<KetQua>()
                .HasOne<BaiLam>(k => k.BaiLam)
                .WithOne(b => b.KetQua)
                .HasForeignKey<KetQua>(k => k.baiLamId);

            // Thêm các cấu hình mối quan hệ khác tương tự nếu cần
        }
    }
}
