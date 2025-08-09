using System;
using System.Collections.Generic;

namespace TaskService.Models.Entities
{
    public class YeuCauTaoDeThi
    {
        // Thuộc tính
        public String id { get; set; }
        public String giaoVienId { get; set; }
        public String monHoc { get; set; }
        public int soLuongCauHoi { get; set; }
        public float tongDiem { get; set; }
        public int thoiGianLam { get; set; }

        // Có thể thêm thuộc tính cho mức độ khó của câu hỏi
        public String doKho { get; set; } // Ví dụ: "De", "TrungBinh", "Kho"

        // Các phương thức (Methods)

        /// <summary>
        /// Tạo một đề thi mới từ yêu cầu này.
        /// </summary>
        /// <param name="tatCaCauHoi">Danh sách tất cả các câu hỏi có sẵn.</param>
        /// <returns>Đối tượng DeThi được tạo.</returns>
        public DeThi TaoDeThi(ICollection<CauHoi> tatCaCauHoi)
        {
            var deThi = new DeThi
            {
                id = Guid.NewGuid().ToString(),
                tieuDe = "Đề thi tự động cho môn " + this.monHoc,
                monHoc = this.monHoc,
                thoiGianLam = this.thoiGianLam,
                tongDiem = this.tongDiem
            };

            // Lọc câu hỏi theo môn học và độ khó
            var cauHoiPhuHop = tatCaCauHoi.Where(c => c.monHoc == this.monHoc && c.doKho == this.doKho).ToList();

            // Tạo ngẫu nhiên câu hỏi cho đề thi
            deThi.taoNgauNhien(cauHoiPhuHop, this.soLuongCauHoi);

            return deThi;
        }
    }
}
