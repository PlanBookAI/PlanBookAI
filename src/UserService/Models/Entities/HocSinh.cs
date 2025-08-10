using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskService.Models.Entities
{
    [Table("students", Schema = "students")]
    public class HocSinh
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Required]
        [StringLength(255)]
        [Column("full_name")]
        public string HoTen { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        [Column("student_code")]
        public string MaHocSinh { get; set; } = string.Empty;

        // ... các thuộc tính khác ...
    }
}