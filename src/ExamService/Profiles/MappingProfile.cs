using AutoMapper;
using ExamService.Models.DTOs;
using ExamService.Models.Entities;
using System.Text.Json;

namespace ExamService.Profiles;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // CauHoi Mappings
        CreateMap<CauHoi, CauHoiResponseDTO>();
        CreateMap<CauHoiRequestDTO, CauHoi>();
        CreateMap<LuaChon, LuaChonResponseDTO>();
        CreateMap<LuaChonRequestDTO, LuaChon>();

        // DeThi Mappings
        CreateMap<DeThi, DeThiResponseDTO>()
            .ForMember(dest => dest.SoLuongCauHoi, opt => opt.MapFrom(src => src.ExamQuestions.Count));
        CreateMap<DeThiRequestDTO, DeThi>();
        CreateMap<ExamQuestion, DeThiCauHoiResponseDTO>()
            .ForMember(dest => dest.CauHoi, opt => opt.MapFrom(src => src.CauHoi));

        // MauDeThi Mappings
        CreateMap<MauDeThi, MauDeThiResponseDTO>()
            .ForMember(dest => dest.CauTruc,
                       opt => opt.MapFrom(src => JsonSerializer.Deserialize<List<YeuCauCauHoiDTO>>(src.CauTruc, (JsonSerializerOptions?)null)));

        CreateMap<MauDeThiRequestDTO, MauDeThi>()
            .ForMember(dest => dest.CauTruc,
                       opt => opt.MapFrom(src => JsonSerializer.Serialize(src.CauTruc, (JsonSerializerOptions?)null)));

        // Mapping cho các DTO tạo đề thi
        CreateMap<TaoDeThiBaseDTO, TaoDeThiTuNganHangDTO>();
        CreateMap<TaoDeThiNgauNhienDTO, TaoDeThiTuNganHangDTO>();
        CreateMap<string, List<YeuCauCauHoiDTO>>().ConvertUsing(new JsonStringToListConverter<YeuCauCauHoiDTO>());
    }
}

public class JsonStringToListConverter<T> : ITypeConverter<string, List<T>>
{
    public List<T> Convert(string source, List<T> destination, ResolutionContext context)
    {
        if (string.IsNullOrEmpty(source))
        {
            return new List<T>();
        }
        return JsonSerializer.Deserialize<List<T>>(source, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<T>();
    }
}