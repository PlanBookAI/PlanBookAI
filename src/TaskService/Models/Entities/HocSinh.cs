using System;
using System.Collections.Generic;
using System.Linq;

namespace TaskService.Models.Entities
{
    public class HocSinh
    {
        // Thuộc tính (Attributes)
        public String id { get; set; }
        public String hoTen { get; set; }
        public String maSo { get; set; }
        public String giaoVienId { get; set; }
        public float diemTrungBinh { get; set; }

        // Navigation properties để liên kết với các kết quả của học sinh
        public virtual ICollection<KetQua> KetQuas { get; set; }
        
        // Navigation property cho relationship với BaiLam (1:N)
        public virtual ICollection<BaiLam> BaiLams { get; set; }

        // Các phương thức (Methods)

        /// <summary>
        /// Cập nhật kết quả mới cho học sinh và tính lại điểm trung bình.
        /// </summary>
        /// <param name="ketQuaMoi">Kết quả mới của một bài thi.</param>
        public void capNhatKetQua(KetQua ketQuaMoi)
        {
            if (this.KetQuas == null)
            {
                this.KetQuas = new List<KetQua>();
            }

            // Thêm kết quả mới vào danh sách
            this.KetQuas.Add(ketQuaMoi);

            // Tính toán lại điểm trung bình
            if (this.KetQuas.Any())
            {
                this.diemTrungBinh = this.KetQuas.Average(kq => kq.diem);
            }
            else
            {
                this.diemTrungBinh = 0;
            }

            Console.WriteLine($"Đã cập nhật kết quả và điểm trung bình mới cho học sinh '{this.hoTen}': {this.diemTrungBinh}");
        }

        /// <summary>
        /// Tính toán và trả về trình độ (proficiency) của học sinh dựa trên điểm trung bình.
        /// </summary>
        /// <returns>Chuỗi mô tả trình độ của học sinh.</returns>
        public string tinhProficiency()
        {
            if (this.diemTrungBinh >= 9.0)
            {
                return "Xuất sắc";
            }
            else if (this.diemTrungBinh >= 8.0)
            {
                return "Giỏi";
            }
            else if (this.diemTrungBinh >= 6.5)
            {
                return "Khá";
            }
            else if (this.diemTrungBinh >= 5.0)
            {
                return "Trung bình";
            }
            else
            {
                return "Yếu";
            }
        }
    }
}
