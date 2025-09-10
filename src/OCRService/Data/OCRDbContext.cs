using Microsoft.EntityFrameworkCore;
using OCRService.Models.Entities;

namespace OCRService.Data
{
    /// <summary>
    /// DbContext cho OCRService
    /// </summary>
    public class OCRDbContext : DbContext
    {
        public OCRDbContext(DbContextOptions<OCRDbContext> options) : base(options)
        {
        }

        // === OCR ENTITIES ===

        /// <summary>
        /// OCR Requests (mapping với students.answer_sheets)
        /// </summary>
        public DbSet<OCRRequest> OCRRequests { get; set; }

        /// <summary>
        /// OCR Results (mapping với students.student_results)
        /// </summary>
        public DbSet<OCRResult> OCRResults { get; set; }

        /// <summary>
        /// OCR Rate Limits (mapping với users.ocr_rate_limits)
        /// </summary>
        public DbSet<OCRRateLimit> OCRRateLimits { get; set; }

        // === PLACEHOLDER ENTITIES ===

        /// <summary>
        /// Students (placeholder - sẽ được tạo sau)
        /// </summary>
        public DbSet<Student> Students { get; set; }

        /// <summary>
        /// Exams (placeholder - sẽ được tạo sau)
        /// </summary>
        public DbSet<Exam> Exams { get; set; }

        /// <summary>
        /// Users (placeholder - sẽ được tạo sau)
        /// </summary>
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // === OCRRequest CONFIGURATION ===

            modelBuilder.Entity<OCRRequest>(entity =>
            {
                entity.ToTable("answer_sheets", "students");
                
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                
                entity.Property(e => e.StudentId).HasColumnName("student_id").IsRequired();
                entity.Property(e => e.ExamId).HasColumnName("exam_id").IsRequired();
                entity.Property(e => e.ImageUrl).HasColumnName("image_url").HasMaxLength(500);
                entity.Property(e => e.OcrResult).HasColumnName("ocr_result");
                entity.Property(e => e.OcrStatus).HasColumnName("ocr_status").HasMaxLength(50).IsRequired();
                entity.Property(e => e.OcrAccuracy).HasColumnName("ocr_accuracy").HasColumnType("numeric(5,2)");
                entity.Property(e => e.ProcessedAt).HasColumnName("processed_at");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();
                
                // OCR columns bổ sung
                entity.Property(e => e.OcrRequestId).HasColumnName("ocr_request_id");
                entity.Property(e => e.OcrProcessingTime).HasColumnName("ocr_processing_time");
                entity.Property(e => e.OcrProvider).HasColumnName("ocr_provider").HasMaxLength(50);
                entity.Property(e => e.OcrStartedAt).HasColumnName("ocr_started_at");
                entity.Property(e => e.OcrCompletedAt).HasColumnName("ocr_completed_at");
                entity.Property(e => e.OcrError).HasColumnName("ocr_error");
                entity.Property(e => e.RetryCount).HasColumnName("retry_count").HasDefaultValue(0);
                entity.Property(e => e.MaxRetries).HasColumnName("max_retries").HasDefaultValue(3);
                
                // Fallback columns
                entity.Property(e => e.FallbackStatus).HasColumnName("fallback_status").HasMaxLength(50);
                entity.Property(e => e.FallbackReason).HasColumnName("fallback_reason");
                entity.Property(e => e.ManualGradingRequired).HasColumnName("manual_grading_required").HasDefaultValue(false);

                // Indexes
                entity.HasIndex(e => e.StudentId).HasDatabaseName("ix_answer_sheets_student_id");
                entity.HasIndex(e => e.ExamId).HasDatabaseName("ix_answer_sheets_exam_id");
                entity.HasIndex(e => e.OcrStatus).HasDatabaseName("ix_answer_sheets_ocr_status");
                entity.HasIndex(e => e.CreatedAt).HasDatabaseName("ix_answer_sheets_created_at");
                entity.HasIndex(e => e.OcrRequestId).HasDatabaseName("ix_answer_sheets_ocr_request_id");
                entity.HasIndex(e => e.FallbackStatus).HasDatabaseName("ix_answer_sheets_fallback_status");

                // Relationships
                entity.HasOne(e => e.Student)
                    .WithMany()
                    .HasForeignKey(e => e.StudentId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Exam)
                    .WithMany()
                    .HasForeignKey(e => e.ExamId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // === OCRResult CONFIGURATION ===

            modelBuilder.Entity<OCRResult>(entity =>
            {
                entity.ToTable("student_results", "students");
                
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                
                entity.Property(e => e.StudentId).HasColumnName("student_id").IsRequired();
                entity.Property(e => e.ExamId).HasColumnName("exam_id").IsRequired();
                entity.Property(e => e.Score).HasColumnName("score").HasColumnType("numeric(5,2)");
                entity.Property(e => e.ActualDuration).HasColumnName("actual_duration");
                entity.Property(e => e.AnswerDetails).HasColumnName("answer_details");
                entity.Property(e => e.GradingMethod).HasColumnName("grading_method").HasMaxLength(50).IsRequired();
                entity.Property(e => e.Notes).HasColumnName("notes");
                entity.Property(e => e.ExamDate).HasColumnName("exam_date");
                entity.Property(e => e.GradedAt).HasColumnName("graded_at").IsRequired();
                
                // OCR columns bổ sung
                entity.Property(e => e.OcrResultId).HasColumnName("ocr_result_id");
                entity.Property(e => e.OcrConfidence).HasColumnName("ocr_confidence").HasColumnType("numeric(3,2)");
                entity.Property(e => e.FallbackNote).HasColumnName("fallback_note");

                // Indexes
                entity.HasIndex(e => e.StudentId).HasDatabaseName("ix_student_results_student_id");
                entity.HasIndex(e => e.ExamId).HasDatabaseName("ix_student_results_exam_id");
                entity.HasIndex(e => e.GradingMethod).HasDatabaseName("ix_student_results_grading_method");
                entity.HasIndex(e => e.GradedAt).HasDatabaseName("ix_student_results_graded_at");
                entity.HasIndex(e => e.OcrResultId).HasDatabaseName("ix_student_results_ocr_result_id");

                // Relationships
                entity.HasOne(e => e.Student)
                    .WithMany()
                    .HasForeignKey(e => e.StudentId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Exam)
                    .WithMany()
                    .HasForeignKey(e => e.ExamId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // === OCRRateLimit CONFIGURATION ===

            modelBuilder.Entity<OCRRateLimit>(entity =>
            {
                entity.ToTable("ocr_rate_limits", "users");
                
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                
                entity.Property(e => e.TeacherId).HasColumnName("teacher_id").IsRequired();
                entity.Property(e => e.RequestDate).HasColumnName("request_date").IsRequired();
                entity.Property(e => e.RequestCount).HasColumnName("request_count").HasDefaultValue(0);
                entity.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();
                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").IsRequired();

                // Indexes
                entity.HasIndex(e => e.TeacherId).HasDatabaseName("ix_ocr_rate_limits_teacher_id");
                entity.HasIndex(e => e.RequestDate).HasDatabaseName("ix_ocr_rate_limits_request_date");
                entity.HasIndex(e => new { e.TeacherId, e.RequestDate }).HasDatabaseName("ix_ocr_rate_limits_teacher_date");

                // Relationships
                entity.HasOne(e => e.Teacher)
                    .WithMany()
                    .HasForeignKey(e => e.TeacherId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // === PLACEHOLDER ENTITIES CONFIGURATION ===

            modelBuilder.Entity<Student>(entity =>
            {
                entity.ToTable("students", "users");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.FullName).HasColumnName("full_name").HasMaxLength(255).IsRequired();
            });

            modelBuilder.Entity<Exam>(entity =>
            {
                entity.ToTable("exams", "assessment");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Title).HasColumnName("title").HasMaxLength(255).IsRequired();
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users", "users");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Email).HasColumnName("email").HasMaxLength(255).IsRequired();
                entity.Property(e => e.Role).HasColumnName("role").HasMaxLength(50).IsRequired();
            });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Fallback configuration nếu không được inject từ DI
                optionsBuilder.UseNpgsql("Host=localhost;Database=planbookai;Username=postgres;Password=postgres");
            }
        }
    }
}
