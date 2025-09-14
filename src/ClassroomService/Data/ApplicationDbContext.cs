using Microsoft.EntityFrameworkCore;
using ClassroomService.Models.Entities;

namespace ClassroomService.Data
{
    /// <summary>
    /// Database context for the Classroom Service application
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of ApplicationDbContext
        /// </summary>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        /// <summary>
        /// Gets or sets the Classes DbSet
        /// </summary>
        public DbSet<Classes> Classes { get; set; }
        
        /// <summary>
        /// Gets or sets the Students DbSet
        /// </summary>
        public DbSet<Students> Students { get; set; }
        
        /// <summary>
        /// Gets or sets the StudentResults DbSet
        /// </summary>
        public DbSet<StudentResults> StudentResults { get; set; }
        
        /// <summary>
        /// Gets or sets the AnswerSheets DbSet
        /// </summary>
        public DbSet<AnswerSheets> AnswerSheets { get; set; }

        /// <summary>
        /// Configures the model relationships and constraints
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Students entity
            modelBuilder.Entity<Students>(entity =>
            {
                entity.Property(e => e.Gender).HasConversion<string>();
                entity.HasOne(s => s.Class)
                    .WithMany(c => c.Students)
                    .HasForeignKey(s => s.ClassId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Configure indexes
                entity.HasIndex(s => s.StudentCode).IsUnique();
                entity.HasIndex(s => new { s.ClassId, s.OwnerTeacherId });
            });

            // Configure StudentResults entity
            modelBuilder.Entity<StudentResults>(entity =>
            {
                entity.Property(e => e.GradingMethod).HasConversion<string>();
                entity.Property(e => e.Score).HasPrecision(5, 2);
                entity.HasOne(sr => sr.Student)
                    .WithMany(s => s.StudentResults)
                    .HasForeignKey(sr => sr.StudentId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Configure indexes
                entity.HasIndex(sr => new { sr.StudentId, sr.ExamId });
            });

            // Configure AnswerSheets entity
            modelBuilder.Entity<AnswerSheets>(entity =>
            {
                entity.Property(e => e.OcrStatus).HasConversion<string>();
                entity.Property(e => e.OcrAccuracy).HasPrecision(5, 2);
                entity.HasOne(asheet => asheet.Student)
                    .WithMany(s => s.AnswerSheets)
                    .HasForeignKey(asheet => asheet.StudentId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Configure indexes
                entity.HasIndex(asheet => new { asheet.StudentId, asheet.ExamId });
            });
        }
    }
}