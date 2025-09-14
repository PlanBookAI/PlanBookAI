using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassroomService.Models.Entities
{
    /// <summary>
    /// Class entity representing school classes
    /// </summary>
    [Table("classes")]
    public class Classes
    {
        /// <summary>
        /// Class ID
        /// </summary>
        [Key]
        public int Id { get; set; }
        
        /// <summary>
        /// Class name
        /// </summary>
        [Required, MaxLength(200)]
        public required string Name { get; set; }
        
        /// <summary>
        /// Grade level (10-12)
        /// </summary>
        [Required]
        public int Grade { get; set; } 
        
        /// <summary>
        /// Number of students in class
        /// </summary>
        [Column("student_count")]
        public int StudentCount { get; set; }
        
        /// <summary>
        /// Homeroom teacher ID
        /// </summary>
        [Column("homeroom_teacher_id")]
        public int HomeroomTeacherId { get; set; }
        
        /// <summary>
        /// Academic year (e.g., 2023-2024)
        /// </summary>
        [Required, MaxLength(50)]
        [Column("academic_year")]
        public required string AcademicYear { get; set; }
        
        /// <summary>
        /// Whether the class is currently active
        /// </summary>
        [Column("is_active")]
        public bool IsActive { get; set; } = true;
        
        /// <summary>
        /// Creation timestamp
        /// </summary>
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        /// <summary>
        /// Last update timestamp
        /// </summary>
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        /// <summary>
        /// Navigation property for students in this class
        /// </summary>
        public virtual ICollection<Students> Students { get; set; } = new List<Students>();
    }
}