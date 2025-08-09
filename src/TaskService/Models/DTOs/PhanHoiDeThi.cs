using System;
using System.Collections.Generic;

namespace TaskService.Models.Entities
{
    // Lớp mô tả phản hồi của học sinh đối với một đề thi
    public class PhanHoiDeThi
    {
        // Thuộc tính
        public String id { get; set; }
        public String deThiId { get; set; }
        public String hocSinhId { get; set; }
        public DateTime thoiGianNopBai { get; set; }

        // Navigation properties để lưu danh sách câu trả lời chi tiết
        public virtual ICollection<CauTraLoi> CauTraLois { get; set; }

        // Phương thức (Methods)

        /// <summary>
        /// Thêm một câu trả lời vào phản hồi.
        /// </summary>
        /// <param name="cauTraLoi">Câu trả lời của học sinh.</param>
        public void themCauTraLoi(CauTraLoi cauTraLoi)
        {
            if (this.CauTraLois == null)
            {
                this.CauTraLois = new List<CauTraLoi>();
            }
            this.CauTraLois.Add(cauTraLoi);
            Console.WriteLine($"Đã thêm câu trả lời cho câu hỏi '{cauTraLoi.cauHoiId}' vào bài làm.");
        }

        /// <summary>
        /// Tính toán tổng điểm của phản hồi dựa trên đáp án đúng.
        /// </summary>
        /// <param name="deThi">Đề thi tương ứng với phản hồi.</param>
        /// <returns>Tổng điểm của bài làm.</returns>
        public float tinhDiem(DeThi deThi)
        {
            float tongDiem = 0;
            if (this.CauTraLois != null && deThi.CauHois != null)
            {
                foreach (var cauTraLoi in this.CauTraLois)
                {
                    // Tìm câu hỏi tương ứng trong đề thi
                    var cauHoi = deThi.CauHois.FirstOrDefault(c => c.id == cauTraLoi.cauHoiId);

                    if (cauHoi != null && cauHoi.dapAnDung == cauTraLoi.dapAnHocSinh)
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
        public void ghiNhanThoiGianNopBai()
        {
            this.thoiGianNopBai = DateTime.Now;
            Console.WriteLine($"Thời gian nộp bài đã được ghi lại: {this.thoiGianNopBai}");
        }
    }

    // Lớp mô tả chi tiết câu trả lời của học sinh cho từng câu hỏi
    public class CauTraLoi
    {
        public String id { get; set; }
        public String cauHoiId { get; set; }
        public String dapAnHocSinh { get; set; }

        // Navigation property
        public virtual PhanHoiDeThi PhanHoiDeThi { get; set; }
    }
}
