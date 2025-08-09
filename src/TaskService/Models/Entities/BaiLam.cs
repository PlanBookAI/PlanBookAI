using System;
using System.Collections.Generic;

namespace TaskService.Models.Entities
{
    public class BaiLam
    {
        // Thuộc tính (Attributes)
        public String id { get; set; }
        public String hocSinhId { get; set; }
        public String deThiId { get; set; }
        public DateTime thoiGianNopBai { get; set; }

        // Navigation properties để liên kết với các entities khác
        public virtual HocSinh HocSinh { get; set; } // Mối quan hệ N-1 với HocSinh
        public virtual DeThi DeThi { get; set; } // Mối quan hệ N-1 với DeThi
        public virtual KetQua KetQua { get; set; } // Mối quan hệ 1-1 với KetQua

        // Có thể thêm một collection các câu trả lời chi tiết
        public virtual ICollection<CauTraLoi> CauTraLois { get; set; }

        // Các phương thức (Methods)

        /// <summary>
        /// Ghi lại thời gian nộp bài của học sinh.
        /// </summary>
        public void ghiNhanThoiGianNopBai()
        {
            this.thoiGianNopBai = DateTime.Now;
            Console.WriteLine($"Bài làm '{this.id}' của học sinh '{this.hocSinhId}' đã được nộp vào lúc {this.thoiGianNopBai}.");
        }

        /// <summary>
        /// Thêm một câu trả lời của học sinh vào bài làm.
        /// </summary>
        /// <param name="cauTraLoi">Câu trả lời chi tiết.</param>
        public void themCauTraLoi(CauTraLoi cauTraLoi)
        {
            if (this.CauTraLois == null)
            {
                this.CauTraLois = new List<CauTraLoi>();
            }
            this.CauTraLois.Add(cauTraLoi);
            Console.WriteLine($"Đã thêm câu trả lời cho câu hỏi '{cauTraLoi.cauHoiId}' vào bài làm.");
        }
    }
}
