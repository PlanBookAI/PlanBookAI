using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassroomService.Models.Entities
{
    public enum Gender
    {
        MALE,
        FEMALE,
        OTHER
    }

    [Table("students")]
    public class Students
    {
        [Key]
        public int Id { get; set; }
        
        [Required, MaxLength(200)]
        [Column("full_name")]
        public string FullName { get; set; }
        
        [Required, MaxLength(50)]
        [Column("student_code")]
        public string StudentCode { get; set; }
        
        [Column("birth_date")]
        public DateOnly BirthDate { get; set; }
        
        [Required]
        public Gender Gender { get; set; }
        
        [Column("class_id")]
        public int ClassId { get; set; }
        
        [Column("owner_teacher_id")]
        public int OwnerTeacherId { get; set; }
        
        [Column("is_active")]
        public bool IsActive { get; set; } = true;
        
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Thuộc tính điều hướng
        [ForeignKey("ClassId")]
        public virtual Classes Class { get; set; }
        
        public virtual ICollection<StudentResults> StudentResults { get; set; }
        public virtual ICollection<AnswerSheets> AnswerSheets { get; set; }
    }
}