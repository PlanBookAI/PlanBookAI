namespace PlanService.Models.Enums
{
    /// <summary>
    /// Các môn học được hỗ trợ trong hệ thống PlanbookAI
    /// Phase 1: Chỉ hỗ trợ môn Hóa học
    /// </summary>
    public enum MonHoc
    {
        HoaHoc = 1
    }

    /// <summary>
    /// Extension methods cho MonHoc enum
    /// </summary>
    public static class MonHocExtensions
    {
        /// <summary>
        /// Chuyển enum thành string để lưu database
        /// </summary>
        public static string ToStringValue(this MonHoc monHoc)
        {
            return monHoc.ToString();
        }

        /// <summary>
        /// Chuyển string từ database thành enum
        /// </summary>
        public static MonHoc FromString(string value)
        {
            return Enum.TryParse<MonHoc>(value, true, out var result)
                ? result
                : MonHoc.HoaHoc;
        }

        /// <summary>
        /// Lấy tên hiển thị tiếng Việt
        /// </summary>
        public static string GetDisplayName(this MonHoc monHoc)
        {
            return monHoc switch
            {
                MonHoc.HoaHoc => "Hóa học",
                _ => "Không xác định"
            };
        }

        /// <summary>
        /// Lấy mã môn học (để URL, API)
        /// </summary>
        public static string GetSubjectCode(this MonHoc monHoc)
        {
            return monHoc switch
            {
                MonHoc.HoaHoc => "CHEM",
                _ => "UNKNOWN"
            };
        }
    }
}