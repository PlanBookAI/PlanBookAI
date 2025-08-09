using System;
using System.Collections.Generic;

namespace TaskService.Models.Entities
{
    public class KetQua
    {
        // Thuộc tính (Attributes)
        public String id { get; set; }
        public String hocSinhId { get; set; }
        public String deThiId { get; set; }
        public float diem { get; set; }
        public int soCauDung { get; set; }
        public String chiTietDapAn { get; set; }

        // Navigation properties cho Entity Framework Core
        // Mỗi kết quả liên kết với một Học sinh
        public virtual HocSinh HocSinh { get; set; }
        // Mỗi kết quả liên kết với một Đề thi
        public virtual DeThi DeThi { get; set; }

        // Các phương thức (Methods)

        /// <summary>
        /// Tính toán điểm dựa trên số câu trả lời đúng và tổng điểm của đề thi.
        /// </summary>
        /// <param name="tongSoCau">Tổng số câu hỏi trong đề thi.</param>
        /// <param name="tongDiemDeThi">Tổng điểm tối đa của đề thi.</param>
        public void tinhDiem(int tongSoCau, float tongDiemDeThi)
        {
            if (tongSoCau > 0)
            {
                // Tính điểm theo tỷ lệ
                this.diem = (float)this.soCauDung / tongSoCau * tongDiemDeThi;
                Console.WriteLine($"Điểm của học sinh là: {this.diem}");
            }
            else
            {
                this.diem = 0;
                Console.WriteLine("Đề thi không có câu hỏi nào. Điểm là 0.");
            }
        }

        /// <summary>
        /// Tạo một báo cáo tóm tắt kết quả của học sinh.
        /// </summary>
        /// <returns>Một chuỗi chứa báo cáo tóm tắt.</returns>
        public string taobaoCao()
        {
            // Có thể sử dụng chiTietDapAn để tạo báo cáo chi tiết hơn
            var baoCao = new System.Text.StringBuilder();
            baoCao.AppendLine("--- Báo cáo kết quả ---");
            baoCao.AppendLine($"ID Kết quả: {this.id}");
            baoCao.AppendLine($"ID Học sinh: {this.hocSinhId}");
            baoCao.AppendLine($"ID Đề thi: {this.deThiId}");
            baoCao.AppendLine($"Điểm số: {this.diem}");
            baoCao.AppendLine($"Số câu trả lời đúng: {this.soCauDung}");
            baoCao.AppendLine("--- Kết thúc báo cáo ---");

            Console.WriteLine(baoCao.ToString());
            return baoCao.ToString();
        }
    }
}
