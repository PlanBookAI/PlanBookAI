using Mapster;
using ExamService.Models.DTOs;
using ExamService.Models.Entities;
using System.Text.Json;

namespace ExamService.Profiles;

public static class MapsterConfig
{
    public static void Configure()
    {
        // CauHoi Mappings
        TypeAdapterConfig<CauHoi, CauHoiResponseDTO>.NewConfig()
            .Map(dest => dest.LuaChons, src => src.LuaChons);

        TypeAdapterConfig<CauHoiRequestDTO, CauHoi>.NewConfig()
            .IgnoreNullValues(true)
            .Map(dest => dest.TaoLuc, src => DateTime.UtcNow)
            .Map(dest => dest.CapNhatLuc, src => DateTime.UtcNow)
            .Map(dest => dest.TrangThai, src => "ACTIVE");

        // LuaChon Mappings
        TypeAdapterConfig<LuaChon, LuaChonResponseDTO>.NewConfig();
        
        TypeAdapterConfig<LuaChonRequestDTO, LuaChon>.NewConfig()
            .IgnoreNullValues(true);

        // DeThi Mappings
        TypeAdapterConfig<DeThi, DeThiResponseDTO>.NewConfig()
            .Map(dest => dest.SoLuongCauHoi, src => src.ExamQuestions.Count);

        TypeAdapterConfig<DeThiRequestDTO, DeThi>.NewConfig()
            .IgnoreNullValues(true)
            .Map(dest => dest.TaoLuc, src => DateTime.UtcNow)
            .Map(dest => dest.CapNhatLuc, src => DateTime.UtcNow)
            .Map(dest => dest.TrangThai, src => "DRAFT")
            .Map(dest => dest.NguoiTaoId, src => Guid.Empty); // Sẽ được set trong service

        // ExamQuestion Mappings
        TypeAdapterConfig<ExamQuestion, DeThiCauHoiResponseDTO>.NewConfig()
            .Map(dest => dest.CauHoi, src => src.CauHoi)
            .Map(dest => dest.ThuTu, src => src.ThuTu)
            .Map(dest => dest.Diem, src => src.Diem);

        // MauDeThi Mappings với JSON serialization
        TypeAdapterConfig<MauDeThi, MauDeThiResponseDTO>.NewConfig()
            .Map(dest => dest.CauTruc, src => 
                string.IsNullOrEmpty(src.CauTruc) 
                    ? new List<YeuCauCauHoiDTO>() 
                    : JsonSerializer.Deserialize<List<YeuCauCauHoiDTO>>(src.CauTruc, (JsonSerializerOptions?)null) ?? new List<YeuCauCauHoiDTO>());

        TypeAdapterConfig<MauDeThiRequestDTO, MauDeThi>.NewConfig()
            .IgnoreNullValues(true)
            .Map(dest => dest.CauTruc, src => 
                src.CauTruc != null 
                    ? JsonSerializer.Serialize(src.CauTruc, (JsonSerializerOptions?)null) 
                    : "[]")
            .Map(dest => dest.TaoLuc, src => DateTime.UtcNow)
            .Map(dest => dest.CapNhatLuc, src => DateTime.UtcNow)
            .Map(dest => dest.NguoiTaoId, src => Guid.Empty); // Sẽ được set trong service

        // Mapping cho các DTO tạo đề thi
        TypeAdapterConfig<TaoDeThiBaseDTO, TaoDeThiTuNganHangDTO>.NewConfig();
        TypeAdapterConfig<TaoDeThiNgauNhienDTO, TaoDeThiTuNganHangDTO>.NewConfig();
        TypeAdapterConfig<TaoDeThiTuDongDTO, TaoDeThiTuNganHangDTO>.NewConfig();
        TypeAdapterConfig<TaoDeThiTuMauDTO, TaoDeThiTuNganHangDTO>.NewConfig();
    }
}
