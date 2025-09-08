using System;

namespace ExamService.MessageContracts
{
    // Sử dụng interface để định nghĩa contract, đây là best practice của MassTransit
    public interface DeThiMoiCreated
    {
        Guid DeThiId { get; }
        string TieuDe { get; }
        Guid NguoiTaoId { get; }
        DateTime Timestamp { get; }
    }

    public interface CauHoiMoiCreated
    {
        Guid CauHoiId { get; }
        Guid NguoiTaoId { get; }
        string MonHoc { get; }
        DateTime Timestamp { get; }
    }

    public interface DeThiPublished
    {
        Guid DeThiId { get; }
        Guid NguoiTaoId { get; }
        DateTime Timestamp { get; }
    }
}