using System;
using System.Collections.Generic;
using System.Linq;

namespace TaskService.Models.Entities
{
    public class DeThi
    {
        // Thuộc tính (Attributes)
        public String id { get; set; }
        public String tieuDe { get; set; }
        public String monHoc { get; set; }
        public int thoiGianLam { get; set; }
        public float tongDiem { get; set; }

        // Navigation properties để liên kết với các câu hỏi trong đề thi
        public virtual ICollection<CauHoi> CauHois { get; set; }
        
        // Navigation property cho relationship với BaiLam (1:N)
        public virtual ICollection<BaiLam> BaiLams { get; set; }

        // Các phương thức (Methods)

        /// <summary>
        /// Thêm một câu hỏi vào đề thi.
        /// </summary>
        /// <param name="cauHoi">Câu hỏi cần thêm.</param>
        public void themCauHoi(CauHoi cauHoi)
        {
            if (this.CauHois == null)
            {
                this.CauHois = new List<CauHoi>();
            }
            this.CauHois.Add(cauHoi);
            Console.WriteLine($"Đã thêm câu hỏi '{cauHoi.noiDung}' vào đề thi.");
        }

        /// <summary>
        /// Tạo ngẫu nhiên một số câu hỏi từ một danh sách có sẵn.
        /// </summary>
        /// <param name="tatCaCauHoi">Danh sách tất cả các câu hỏi có thể.</param>
        /// <param name="soLuongCauHoi">Số lượng câu hỏi cần tạo.</param>
        public void taoNgauNhien(ICollection<CauHoi> tatCaCauHoi, int soLuongCauHoi)
        {
            // Kiểm tra số lượng câu hỏi có sẵn
            if (tatCaCauHoi.Count < soLuongCauHoi)
            {
                Console.WriteLine("Số lượng câu hỏi cần tạo lớn hơn số lượng câu hỏi có sẵn.");
                return;
            }

            // Chọn ngẫu nhiên các câu hỏi
            var random = new Random();
            var cauHoiNgauNhien = tatCaCauHoi.OrderBy(c => random.Next()).Take(soLuongCauHoi).ToList();

            this.CauHois = cauHoiNgauNhien;
            Console.WriteLine($"Đã tạo ngẫu nhiên {soLuongCauHoi} câu hỏi cho đề thi.");
        }

        /// <summary>
        /// Chấm điểm một bài làm và trả về kết quả.
        /// </summary>
        /// <param name="baiLam">Bài làm của học sinh.</param>
        /// <returns>Đối tượng KetQua chứa thông tin điểm số.</returns>
        public KetQua chamDiem(BaiLam baiLam)
        {
            var ketQua = new KetQua();
            ketQua.hocSinhId = baiLam.hocSinhId;
            ketQua.deThiId = this.id;

            // Logic chấm điểm chi tiết sẽ được thực hiện tại đây
            // Giả định BaiLam chứa danh sách các đáp án của học sinh
            // Và chúng ta so sánh với đáp án đúng của CauHois

            ketQua.soCauDung = 0; // Thay thế bằng logic chấm điểm thực tế

            // Tính điểm
            if (this.CauHois != null && this.CauHois.Any())
            {
                float diemMoiCau = this.tongDiem / this.CauHois.Count;
                ketQua.diem = ketQua.soCauDung * diemMoiCau;
            }
            else
            {
                ketQua.diem = 0;
            }

            Console.WriteLine($"Đã chấm điểm bài làm của học sinh '{baiLam.hocSinhId}'. Điểm số: {ketQua.diem}");
            return ketQua;
        }
    }
}
