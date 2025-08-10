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
        // Thêm DbSet cho các entity
        public DbSet<GiaoAn> GiaoAns { get; set; }
        public DbSet<MauGiaoAn> MauGiaoAns { get; set; }
        public DbSet<NoiDungGiaoDuc> NoiDungGiaoDucs { get; set; }
        public DbSet<MucTieu> MucTieus { get; set; }

        public PlanDbContext(DbContextOptions<PlanDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Cấu hình mối quan hệ 1-N giữa GiaoAn và NoiDungGiaoDuc
            modelBuilder.Entity<NoiDungGiaoDuc>()
                .HasOne(nd => nd.GiaoAn) // Mỗi NoiDungGiaoDuc có một GiaoAn
                .WithMany(ga => ga.NoiDungGiaoDucs) // Mỗi GiaoAn có nhiều NoiDungGiaoDuc
                .HasForeignKey(nd => nd.GiaoAnId); // Khóa ngoại là GiaoAnId

            // Cấu hình mối quan hệ 1-N giữa GiaoAn và MucTieu
            modelBuilder.Entity<MucTieu>()
                .HasOne(mt => mt.GiaoAn) // Mỗi MucTieu có một GiaoAn
                .WithMany(ga => ga.MucTieus) // Mỗi GiaoAn có nhiều MucTieu
                .HasForeignKey(mt => mt.GiaoAnId); // Khóa ngoại là GiaoAnId

            base.OnModelCreating(modelBuilder);
        }
    }
}