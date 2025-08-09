using System;
using System.Collections.Generic;

namespace TaskService.Models.Entities
{
    public class TrangThaiDeThi
    {
        // Thuộc tính (Attributes)
        public String id { get; set; }
        public String tenTrangThai { get; set; }
        public String moTa { get; set; }

        // Các phương thức (Methods)

        /// <summary>
        /// Tạo một trạng thái mới cho đề thi.
        /// </summary>
        /// <param name="tenTrangThaiMoi">Tên của trạng thái mới.</param>
        /// <param name="moTaMoi">Mô tả chi tiết về trạng thái.</param>
        public void taoTrangThai(string tenTrangThaiMoi, string moTaMoi)
        {
            this.id = Guid.NewGuid().ToString();
            this.tenTrangThai = tenTrangThaiMoi;
            this.moTa = moTaMoi;
            Console.WriteLine($"Đã tạo trạng thái đề thi mới: '{this.tenTrangThai}'");
        }

        /// <summary>
        /// Cập nhật tên và mô tả của một trạng thái.
        /// </summary>
        /// <param name="tenMoi">Tên mới của trạng thái.</param>
        /// <param name="moTaMoi">Mô tả mới của trạng thái.</param>
        public void capNhatTrangThai(string tenMoi, string moTaMoi)
        {
            this.tenTrangThai = tenMoi;
            this.moTa = moTaMoi;
            Console.WriteLine($"Đã cập nhật trạng thái đề thi thành: '{this.tenTrangThai}'");
        }
    }
}
