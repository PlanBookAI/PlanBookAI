using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassroomService.Models.Entities
{
    [Table("classes")]
    public class Classes
    {
        [Key]
        public int Id { get; set; }
        
        [Required, MaxLength(200)]
        public string Name { get; set; }
        
        [Required]
        public int Grade { get; set; } 
        
        [Column("student_count")]
        public int StudentCount { get; set; }
        
        [Column("homeroom_teacher_id")]
        public int HomeroomTeacherId { get; set; }
        
        [Required, MaxLength(50)]
        [Column("academic_year")]
        public string AcademicYear { get; set; }
        
        [Column("is_active")]
        public bool IsActive { get; set; } = true;
        
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Thuộc tính điều hướng
        public virtual ICollection<Students> Students { get; set; }
    }
}