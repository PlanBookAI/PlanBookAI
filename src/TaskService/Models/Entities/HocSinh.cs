using System;
using System.Collections.Generic;
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
        [Column("student_code")]
        public string MaSo { get; set; } = string.Empty;

        [Required]
        [Column("full_name")]
        public string HoTen { get; set; } = string.Empty;

        [Column("date_of_birth")]
        public DateTime? NgaySinh { get; set; }

        [Column("gender")]
        public string? GioiTinh { get; set; }

        [Column("class_id")]
        public Guid? LopId { get; set; }

        [Required]
        [Column("teacher_id")]
        public Guid GiaoVienId { get; set; }

        [Column("parent_contact")]
        public string? LienHePhuHuynh { get; set; }

        [Column("address")]
        public string? DiaChi { get; set; }

        [Column("is_active")]
        public bool LaHoatDong { get; set; } = true;

        [Column("created_at")]
        public DateTime TaoLuc { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime CapNhatLuc { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual ICollection<KetQua> KetQuas { get; set; }
        public virtual ICollection<BaiLam> BaiLams { get; set; }

        public HocSinh()
        {
            KetQuas = new List<KetQua>();
            BaiLams = new List<BaiLam>();
        }
    }
}