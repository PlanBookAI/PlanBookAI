using System;
using System.Collections.Generic;
using System.Linq;
using TaskService.Models.Entities;

namespace TaskService.Models.DTOs
{
    public class YeuCauTaoDeThi
    {
        // Thuộc tính
        public String Id { get; set; }
        public String GiaoVienId { get; set; }
        public String MonHoc { get; set; }
        public int SoLuongCauHoi { get; set; }
        public float TongDiem { get; set; }
        public int ThoiGianLam { get; set; }

        // Có thể thêm thuộc tính cho mức độ khó của câu hỏi
        public String DoKho { get; set; } // Ví dụ: "De", "TrungBinh", "Kho"

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
                tieuDe = "Đề thi tự động cho môn " + this.MonHoc,
                monHoc = this.MonHoc,
                thoiGianLam = this.ThoiGianLam,
                tongDiem = this.TongDiem
            };

            // Lọc câu hỏi theo môn học và độ khó
            var cauHoiPhuHop = tatCaCauHoi.Where(c => c.monHoc == this.MonHoc && c.doKho == this.DoKho).ToList();

            // Tạo ngẫu nhiên câu hỏi cho đề thi
            deThi.taoNgauNhien(cauHoiPhuHop, this.SoLuongCauHoi);

            return deThi;
        }
    }
}
