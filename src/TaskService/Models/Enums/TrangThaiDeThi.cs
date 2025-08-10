using System;
using System.Collections.Generic;

namespace TaskService.Models.Enums
{
    /// <summary>
    /// Enum định nghĩa các trạng thái của đề thi
    /// </summary>
    public enum TrangThaiDeThi
    {
        /// <summary>
        /// Đề thi đang được tạo
        /// </summary>
        DangTao = 1,
        
        /// <summary>
        /// Đề thi đã hoàn thành và sẵn sàng sử dụng
        /// </summary>
        HoanThanh = 2,
        
        /// <summary>
        /// Đề thi đang được sử dụng trong kỳ thi
        /// </summary>
        DangSuDung = 3,
        
        /// <summary>
        /// Đề thi đã kết thúc và không còn sử dụng
        /// </summary>
        KetThuc = 4,
        
        /// <summary>
        /// Đề thi bị vô hiệu hóa
        /// </summary>
        VoHieuHoa = 5
    }
}
