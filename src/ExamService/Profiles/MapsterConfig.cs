using Mapster;
using ExamService.Models.DTOs;
using ExamService.Models.Entities;
using System.Text.Json;
using System.Linq;

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
            .Map(dest => dest.TrangThai, src => "ACTIVE")
            .Map(dest => dest.LuaChons, src => src.LuaChons);

        // LuaChon Mappings
        TypeAdapterConfig<LuaChon, LuaChonResponseDTO>.NewConfig();
        
        TypeAdapterConfig<LuaChonRequestDTO, LuaChon>.NewConfig()
            .IgnoreNullValues(true);

        // DeThi Mappings
        TypeAdapterConfig<DeThi, DeThiResponseDTO>.NewConfig()
            .Map(dest => dest.SoLuongCauHoi, src => src.ExamQuestions.Count)
            .Map(dest => dest.CauHois, src => src.ExamQuestions.OrderBy(eq => eq.ThuTu).ToList());

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

        // Mapping từ ExamQuestion sang CauHoiTrongDeThiDTO
        TypeAdapterConfig<ExamQuestion, CauHoiTrongDeThiDTO>.NewConfig()
            .Map(dest => dest.Id, src => src.CauHoi.Id)
            .Map(dest => dest.NoiDung, src => src.CauHoi.NoiDung)
            .Map(dest => dest.MonHoc, src => src.CauHoi.MonHoc)
            .Map(dest => dest.LoaiCauHoi, src => src.CauHoi.LoaiCauHoi)
            .Map(dest => dest.ChuDe, src => src.CauHoi.ChuDe)
            .Map(dest => dest.DoKho, src => src.CauHoi.DoKho)
            .Map(dest => dest.DapAnDung, src => src.CauHoi.DapAnDung)
            .Map(dest => dest.GiaiThich, src => src.CauHoi.GiaiThich)
            .Map(dest => dest.TaoLuc, src => src.CauHoi.TaoLuc)
            .Map(dest => dest.CapNhatLuc, src => src.CauHoi.CapNhatLuc)
            .Map(dest => dest.NguoiTaoId, src => src.CauHoi.NguoiTaoId)
            .Map(dest => dest.LuaChons, src => src.CauHoi.LuaChons)
            .Map(dest => dest.Diem, src => src.Diem)
            .Map(dest => dest.ThuTu, src => src.ThuTu);

        // MauDeThi Mappings với JSON serialization
        TypeAdapterConfig<MauDeThi, MauDeThiResponseDTO>.NewConfig()
            .Map(dest => dest.CauTruc, src => 
                string.IsNullOrEmpty(src.CauTruc) 
                    ? new Dictionary<string, object>() 
                    : JsonSerializer.Deserialize<Dictionary<string, object>>(src.CauTruc, (JsonSerializerOptions?)null) ?? new Dictionary<string, object>());

        TypeAdapterConfig<MauDeThiRequestDTO, MauDeThi>.NewConfig()
            .IgnoreNullValues(true)
            .Map(dest => dest.CauTruc, src => 
                src.CauTruc != null 
                    ? JsonSerializer.Serialize(src.CauTruc, (JsonSerializerOptions?)null) 
                    : null)
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
