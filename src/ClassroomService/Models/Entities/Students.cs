using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassroomService.Models.Entities
{
    /// <summary>
    /// Gender enumeration
    /// </summary>
    public enum Gender
    {
        /// <summary>
        /// Male gender
        /// </summary>
        MALE,
        /// <summary>
        /// Female gender
        /// </summary>
        FEMALE,
        /// <summary>
        /// Other gender
        /// </summary>
        OTHER
    }

    /// <summary>
    /// Student entity representing school students
    /// </summary>
    [Table("students")]
    public class Students
    {
        /// <summary>
        /// Student ID
        /// </summary>
        [Key]
        public int Id { get; set; }
        
        /// <summary>
        /// Student's full name
        /// </summary>
        [Required, MaxLength(200)]
        [Column("full_name")]
        public required string FullName { get; set; }
        
        /// <summary>
        /// Unique student code
        /// </summary>
        [Required, MaxLength(50)]
        [Column("student_code")]
        public required string StudentCode { get; set; }
        
        /// <summary>
        /// Student's birth date
        /// </summary>
        [Column("birth_date")]
        public DateOnly BirthDate { get; set; }
        
        /// <summary>
        /// Student's gender
        /// </summary>
        [Required]
        public Gender Gender { get; set; }
        
        /// <summary>
        /// ID of the class this student belongs to
        /// </summary>
        [Column("class_id")]
        public int ClassId { get; set; }
        
        /// <summary>
        /// ID of the teacher who owns/manages this student
        /// </summary>
        [Column("owner_teacher_id")]
        public int OwnerTeacherId { get; set; }
        
        /// <summary>
        /// Whether the student is currently active
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
        /// Navigation property to the class this student belongs to
        /// </summary>
        [ForeignKey("ClassId")]
        public virtual Classes? Class { get; set; }
        
        /// <summary>
        /// Navigation property to student results
        /// </summary>
        public virtual ICollection<StudentResults> StudentResults { get; set; } = new List<StudentResults>();
        
        /// <summary>
        /// Navigation property to answer sheets
        /// </summary>
        public virtual ICollection<AnswerSheets> AnswerSheets { get; set; } = new List<AnswerSheets>();
    }
}