using Microsoft.EntityFrameworkCore;
using ExamService.Models.Entities;

namespace ExamService.Data;

public class ExamDbContext : DbContext
{
    public ExamDbContext(DbContextOptions<ExamDbContext> options)
        : base(options)
    {
    }

    public DbSet<CauHoi> CauHois { get; set; }
    public DbSet<LuaChon> LuaChons { get; set; }
    public DbSet<DeThi> DeThis { get; set; }
    public DbSet<ExamQuestion> ExamQuestions { get; set; }
    public DbSet<BaiLam> BaiLams { get; set; }
    public DbSet<KetQua> KetQuas { get; set; }
    public DbSet<HocSinh> HocSinhs { get; set; }
    public DbSet<MauDeThi> MauDeThis { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // CauHoi - LuaChon relationship
        modelBuilder.Entity<LuaChon>()
            .HasOne(lc => lc.CauHoi)
            .WithMany(ch => ch.LuaChons)
            .HasForeignKey(lc => lc.CauHoiId)
            .OnDelete(DeleteBehavior.Cascade);

        // CauHoi - ExamQuestion relationship
        modelBuilder.Entity<ExamQuestion>()
            .HasOne(eq => eq.CauHoi)
            .WithMany(ch => ch.ExamQuestions)
            .HasForeignKey(eq => eq.CauHoiId)
            .OnDelete(DeleteBehavior.Cascade);

        // DeThi - ExamQuestion relationship
        modelBuilder.Entity<ExamQuestion>()
            .HasOne(eq => eq.DeThi)
            .WithMany(dt => dt.ExamQuestions)
            .HasForeignKey(eq => eq.DeThiId)
            .OnDelete(DeleteBehavior.Cascade);

        // BaiLam - HocSinh relationship
        modelBuilder.Entity<BaiLam>()
            .HasOne(bl => bl.HocSinh)
            .WithMany(hs => hs.BaiLams)
            .HasForeignKey(bl => bl.HocSinhId)
            .OnDelete(DeleteBehavior.Cascade);

        // BaiLam - DeThi relationship
        modelBuilder.Entity<BaiLam>()
            .HasOne(bl => bl.DeThi)
            .WithMany(dt => dt.BaiLams)
            .HasForeignKey(bl => bl.DeThiId)
            .OnDelete(DeleteBehavior.Cascade);

        // KetQua - BaiLam relationship (1-1)
        modelBuilder.Entity<KetQua>()
            .HasOne(kq => kq.BaiLam)
            .WithOne(bl => bl.KetQua)
            .HasForeignKey<KetQua>(kq => kq.BaiLamId)
            .OnDelete(DeleteBehavior.Cascade);

        // KetQua - HocSinh relationship
        modelBuilder.Entity<KetQua>()
            .HasOne(kq => kq.HocSinh)
            .WithMany(hs => hs.KetQuas)
            .HasForeignKey(kq => kq.HocSinhId)
            .OnDelete(DeleteBehavior.Cascade);

        // KetQua - DeThi relationship
        modelBuilder.Entity<KetQua>()
            .HasOne(kq => kq.DeThi)
            .WithMany()
            .HasForeignKey(kq => kq.DeThiId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        modelBuilder.Entity<CauHoi>()
            .HasIndex(ch => ch.MonHoc);

        modelBuilder.Entity<CauHoi>()
            .HasIndex(ch => ch.DoKho);

        modelBuilder.Entity<DeThi>()
            .HasIndex(dt => dt.MonHoc);

        modelBuilder.Entity<DeThi>()
            .HasIndex(dt => dt.TrangThai);

        modelBuilder.Entity<HocSinh>()
            .HasIndex(hs => hs.MaSo)
            .IsUnique();

        modelBuilder.Entity<HocSinh>()
            .HasIndex(hs => hs.GiaoVienId);
    }
}

