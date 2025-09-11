using Microsoft.EntityFrameworkCore;
using PlanService.Models.Entities;

namespace PlanService.Data
{
    /// <summary>
    /// DbContext là lớp trung tâm để tương tác với cơ sở dữ liệu.
    /// Nó đại diện cho một phiên làm việc với database.
    /// </summary>
    public class PlanDbContext : DbContext
    {
        public DbSet<GiaoAn> GiaoAns { get; set; }
        public DbSet<MauGiaoAn> MauGiaoAns { get; set; }
        public DbSet<ChuDe> ChuDes { get; set; }

        public PlanDbContext(DbContextOptions<PlanDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Một chủ đề có thể có nhiều chủ đề con
            // Mỗi chủ đề con chỉ thuộc về một chủ đề cha
            modelBuilder.Entity<ChuDe>()
                .HasMany(c => c.ChuDeCon)
                .WithOne(c => c.ChuDeCha)
                .HasForeignKey(c => c.ParentId)
                .OnDelete(DeleteBehavior.SetNull);
            // Một chủ đề có thể có nhiều giáo án
            // Mỗi giáo án có thể thuộc về một chủ đề 
            modelBuilder.Entity<GiaoAn>()
                .HasOne(g => g.ChuDe)
                .WithMany()
                .HasForeignKey(g => g.ChuDeId)
                .OnDelete(DeleteBehavior.SetNull);

            // Cấu hình NoiDung là JSONB column
            modelBuilder.Entity<GiaoAn>()
                .Property(e => e.NoiDung)
                .HasColumnType("jsonb");

            base.OnModelCreating(modelBuilder);
        }
    }
}