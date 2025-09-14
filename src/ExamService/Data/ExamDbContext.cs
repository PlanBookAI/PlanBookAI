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
            .HasForeignKey(eq => eq.CauHoiId);

        // DeThi - ExamQuestion relationship
        modelBuilder.Entity<ExamQuestion>()
            .HasOne(eq => eq.DeThi)
            .WithMany(dt => dt.ExamQuestions)
            .HasForeignKey(eq => eq.DeThiId)
            .OnDelete(DeleteBehavior.Cascade);

        // Unique constraints for exam_questions
        modelBuilder.Entity<ExamQuestion>()
            .HasIndex(eq => new { eq.DeThiId, eq.CauHoiId })
            .IsUnique();

        modelBuilder.Entity<ExamQuestion>()
            .HasIndex(eq => new { eq.DeThiId, eq.ThuTu })
            .IsUnique();

        // Check constraints
        modelBuilder.Entity<ExamQuestion>()
            .HasCheckConstraint("exam_questions_points_check", "points > 0");

        modelBuilder.Entity<DeThi>()
            .HasCheckConstraint("exams_duration_minutes_check", "duration_minutes > 0");

        modelBuilder.Entity<DeThi>()
            .HasCheckConstraint("exams_grade_check", "grade = ANY (ARRAY[10, 11, 12])");

        modelBuilder.Entity<DeThi>()
            .HasCheckConstraint("exams_status_check", "status::text = ANY (ARRAY['DRAFT'::character varying, 'PUBLISHED'::character varying, 'COMPLETED'::character varying, 'ARCHIVED'::character varying])");

        modelBuilder.Entity<DeThi>()
            .HasCheckConstraint("exams_total_score_check", "total_score > 0");

        modelBuilder.Entity<LuaChon>()
            .HasCheckConstraint("question_choices_choice_order_check", "choice_order = ANY (ARRAY['A'::bpchar, 'B'::bpchar, 'C'::bpchar, 'D'::bpchar])");

        modelBuilder.Entity<CauHoi>()
            .HasCheckConstraint("questions_difficulty_check", "difficulty::text = ANY (ARRAY['EASY'::character varying, 'MEDIUM'::character varying, 'HARD'::character varying, 'VERY_HARD'::character varying])");

        modelBuilder.Entity<CauHoi>()
            .HasCheckConstraint("questions_status_check", "status::text = ANY (ARRAY['ACTIVE'::character varying, 'INACTIVE'::character varying, 'ARCHIVED'::character varying])");

        modelBuilder.Entity<CauHoi>()
            .HasCheckConstraint("questions_type_check", "type::text = ANY (ARRAY['MULTIPLE_CHOICE'::character varying, 'ESSAY'::character varying, 'SHORT_ANSWER'::character varying, 'TRUE_FALSE'::character varying])");

        // Indexes
        modelBuilder.Entity<CauHoi>()
            .HasIndex(ch => ch.MonHoc)
            .HasDatabaseName("idx_assessment_questions_subject");

        modelBuilder.Entity<CauHoi>()
            .HasIndex(ch => ch.DoKho)
            .HasDatabaseName("idx_assessment_questions_difficulty");

        modelBuilder.Entity<CauHoi>()
            .HasIndex(ch => ch.ChuDe)
            .HasDatabaseName("idx_assessment_questions_topic");

        modelBuilder.Entity<CauHoi>()
            .HasIndex(ch => ch.NguoiTaoId)
            .HasDatabaseName("idx_assessment_questions_created_by");

        modelBuilder.Entity<DeThi>()
            .HasIndex(dt => dt.MonHoc)
            .HasDatabaseName("idx_assessment_exams_subject");

        modelBuilder.Entity<DeThi>()
            .HasIndex(dt => dt.NguoiTaoId)
            .HasDatabaseName("idx_assessment_exams_teacher");

        modelBuilder.Entity<LuaChon>()
            .HasIndex(lc => lc.CauHoiId)
            .HasDatabaseName("idx_assessment_question_choices_question");
    }
}

