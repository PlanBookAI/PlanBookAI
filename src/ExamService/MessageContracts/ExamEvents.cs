namespace ExamService.MessageContracts
{
    /// <summary>
    /// Sự kiện khi một đề thi mới được tạo
    /// </summary>
    public interface DeThiCreated
    {
        Guid DeThiId { get; }
        Guid NguoiTaoId { get; }
        string TieuDe { get; }
        string MonHoc { get; }
        DateTime TaoLuc { get; }
    }
    
    /// <summary>
    /// Sự kiện khi một đề thi mới được tạo (phiên bản cũ)
    /// </summary>
    public interface DeThiMoiCreated
    {
        Guid DeThiId { get; }
        Guid NguoiTaoId { get; }
        string TieuDe { get; }
        string MonHoc { get; }
        DateTime TaoLuc { get; }
    }

    /// <summary>
    /// Sự kiện khi một đề thi được xuất bản
    /// </summary>
    public interface DeThiPublished
    {
        Guid DeThiId { get; }
        Guid NguoiTaoId { get; }
        DateTime Timestamp { get; }
    }

    /// <summary>
    /// Sự kiện khi một câu hỏi mới được tạo
    /// </summary>
    public interface CauHoiCreated
    {
        Guid CauHoiId { get; }
        Guid NguoiTaoId { get; }
        string MonHoc { get; }
        string LoaiCauHoi { get; }
        DateTime TaoLuc { get; }
    }
    
    /// <summary>
    /// Sự kiện khi một câu hỏi mới được tạo (phiên bản cũ)
    /// </summary>
    public interface CauHoiMoiCreated
    {
        Guid CauHoiId { get; }
        Guid NguoiTaoId { get; }
        string MonHoc { get; }
        string LoaiCauHoi { get; }
        DateTime TaoLuc { get; }
    }

    /// <summary>
    /// Sự kiện khi nhiều câu hỏi được import từ Excel
    /// </summary>
    public interface CauHoiImported
    {
        Guid TeacherId { get; }
        int NumberOfQuestions { get; }
        DateTime ImportedAt { get; }
    }
    
    /// <summary>
    /// Sự kiện khi nhiều câu hỏi được xuất ra Excel
    /// </summary>
    public interface CauHoiExported
    {
        Guid TeacherId { get; }
        int NumberOfQuestions { get; }
        DateTime ExportedAt { get; }
    }
    
    /// <summary>
    /// Sự kiện khi một báo cáo thống kê được tạo
    /// </summary>
    public interface ThongKeGenerated
    {
        Guid TeacherId { get; }
        DateTime GeneratedAt { get; }
        string ReportType { get; }
    }
    
    /// <summary>
    /// Sự kiện khi một báo cáo thống kê được xuất ra Excel
    /// </summary>
    public interface ThongKeExported
    {
        Guid TeacherId { get; }
        DateTime ExportedAt { get; }
        string ReportType { get; }
    }
}