namespace PlanService.Models.Enums
{
    /// <summary>
    /// Trạng thái của giáo án trong vòng đời từ tạo đến hoàn thành
    /// </summary>
    public enum TrangThaiGiaoAn
    {
        /// <summary>
        /// Bản thảo - đang soạn, chưa hoàn thành
        /// </summary>
        DRAFT = 0,

        /// <summary>
        /// Đã xuất bản - sẵn sàng để sử dụng giảng dạy
        /// </summary>
        PUBLISHED = 1,

        /// <summary>
        /// Đã hoàn thành - đã sử dụng để dạy xong
        /// </summary>
        COMPLETED = 2,

        /// <summary>
        /// Đã lưu trữ - không sử dụng nữa nhưng giữ lại để tham khảo
        /// </summary>
        ARCHIVED = 3
    }

    /// <summary>
    /// Extension methods cho TrangThaiGiaoAn enum
    /// </summary>
    public static class TrangThaiGiaoAnExtensions
    {
        /// <summary>
        /// Chuyển enum thành string để lưu database
        /// </summary>
        public static string ToStringValue(this TrangThaiGiaoAn trangThai)
        {
            return trangThai.ToString();
        }

        /// <summary>
        /// Chuyển string từ database thành enum
        /// </summary>
        public static TrangThaiGiaoAn FromString(string value)
        {
            return Enum.TryParse<TrangThaiGiaoAn>(value, true, out var result)
                ? result
                : TrangThaiGiaoAn.DRAFT;
        }

        /// <summary>
        /// Lấy mô tả tiếng Việt của trạng thái
        /// </summary>
        public static string GetDisplayName(this TrangThaiGiaoAn trangThai)
        {
            return trangThai switch
            {
                TrangThaiGiaoAn.DRAFT => "Bản thảo",
                TrangThaiGiaoAn.PUBLISHED => "Đã xuất bản",
                TrangThaiGiaoAn.COMPLETED => "Đã hoàn thành",
                TrangThaiGiaoAn.ARCHIVED => "Đã lưu trữ",
                _ => "Không xác định"
            };
        }
    }
}