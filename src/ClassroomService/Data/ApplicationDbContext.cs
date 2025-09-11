using Microsoft.EntityFrameworkCore;
using ClassroomService.Models.Entities;
using System.Text.Json;

namespace ClassroomService.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Classes> Classes { get; set; }
        public DbSet<Students> Students { get; set; }
        public DbSet<StudentResults> StudentResults { get; set; }
        public DbSet<AnswerSheets> AnswerSheets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình thực thể Students
            modelBuilder.Entity<Students>(entity =>
            {
                entity.Property(e => e.Gender).HasConversion<string>();
                entity.HasOne(s => s.Class)
                    .WithMany(c => c.Students)
                    .HasForeignKey(s => s.ClassId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Cấu hình index
                entity.HasIndex(s => s.StudentCode).IsUnique();
                entity.HasIndex(s => new { s.ClassId, s.OwnerTeacherId });
            });

            // Cấu hình thực thể StudentResults
            modelBuilder.Entity<StudentResults>(entity =>
            {
                entity.Property(e => e.GradingMethod).HasConversion<string>();
                entity.Property(e => e.Score).HasPrecision(5, 2);
                entity.HasOne(sr => sr.Student)
                    .WithMany(s => s.StudentResults)
                    .HasForeignKey(sr => sr.StudentId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Cấu hình index
                entity.HasIndex(sr => new { sr.StudentId, sr.ExamId });
            });

            // Cấu hình thực thể AnswerSheets
            modelBuilder.Entity<AnswerSheets>(entity =>
            {
                entity.Property(e => e.OcrStatus).HasConversion<string>();
                entity.Property(e => e.OcrAccuracy).HasPrecision(5, 2);
                entity.HasOne(asheet => asheet.Student)
                    .WithMany(s => s.AnswerSheets)
                    .HasForeignKey(asheet => asheet.StudentId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Cấu hình index
                entity.HasIndex(asheet => new { asheet.StudentId, asheet.ExamId });
            });
        }
    }
}