using System;
using System.Collections.Generic;
using System.Linq;
using TaskService.Models.Entities;

namespace TaskService.Models.DTOs
{
    // Lớp mô tả phản hồi của học sinh đối với một đề thi
    public class PhanHoiDeThi
    {
        // Thuộc tính
        public String Id { get; set; }
        public String DeThiId { get; set; }
        public String HocSinhId { get; set; }
        public DateTime ThoiGianNopBai { get; set; }

        // Navigation properties để lưu danh sách câu trả lời chi tiết
        public virtual ICollection<CauTraLoi> CauTraLois { get; set; }

        // Phương thức (Methods)

        /// <summary>
        /// Thêm một câu trả lời vào phản hồi.
        /// </summary>
        /// <param name="cauTraLoi">Câu trả lời của học sinh.</param>
        public void ThemCauTraLoi(CauTraLoi cauTraLoi)
        {
            if (this.CauTraLois == null)
            {
                this.CauTraLois = new List<CauTraLoi>();
            }
            this.CauTraLois.Add(cauTraLoi);
            Console.WriteLine($"Đã thêm câu trả lời cho câu hỏi '{cauTraLoi.CauHoiId}' vào bài làm.");
        }

        /// <summary>
        /// Tính toán tổng điểm của phản hồi dựa trên đáp án đúng.
        /// </summary>
        /// <param name="deThi">Đề thi tương ứng với phản hồi.</param>
        /// <returns>Tổng điểm của bài làm.</returns>
        public float TinhDiem(DeThi deThi)
        {
            float tongDiem = 0;
            if (this.CauTraLois != null && deThi.CauHois != null)
            {
                foreach (var cauTraLoi in this.CauTraLois)
                {
                    // Tìm câu hỏi tương ứng trong đề thi
                    var cauHoi = deThi.CauHois.FirstOrDefault(c => c.id == cauTraLoi.CauHoiId);

                    if (cauHoi != null && cauHoi.dapAnDung == cauTraLoi.DapAnHocSinh)
                    {
                        // Giả sử mỗi câu hỏi có điểm bằng nhau
                        tongDiem += (deThi.tongDiem / deThi.CauHois.Count);
                    }
                }
            }
            return tongDiem;
        }

        /// <summary>
        /// Ghi lại thời gian nộp bài của học sinh.
        /// </summary>
        public void GhiNhanThoiGianNopBai()
        {
            this.ThoiGianNopBai = DateTime.Now;
            Console.WriteLine($"Thời gian nộp bài đã được ghi lại: {this.ThoiGianNopBai}");
        }
    }

    // Lớp mô tả chi tiết câu trả lời của học sinh cho từng câu hỏi
    public class CauTraLoi
    {
        public String Id { get; set; }
        public String CauHoiId { get; set; }
        public String DapAnHocSinh { get; set; }

        // Navigation property
        public virtual PhanHoiDeThi PhanHoiDeThi { get; set; }
    }
}
