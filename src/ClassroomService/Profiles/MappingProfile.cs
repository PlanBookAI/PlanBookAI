using AutoMapper;
using ClassroomService.Models.DTOs;
using ClassroomService.Models.Entities;

namespace ClassroomService.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Ánh xạ lớp
            CreateMap<CreateClassDto, Classes>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.StudentCount, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Students, opt => opt.Ignore());

            CreateMap<UpdateClassDto, Classes>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.HomeroomTeacherId, opt => opt.Ignore())
                .ForMember(dest => dest.StudentCount, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Students, opt => opt.Ignore());

            CreateMap<Classes, ClassDto>();

            // Ánh xạ học sinh
            CreateMap<CreateStudentDto, Students>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Class, opt => opt.Ignore())
                .ForMember(dest => dest.StudentResults, opt => opt.Ignore())
                .ForMember(dest => dest.AnswerSheets, opt => opt.Ignore());

            CreateMap<UpdateStudentDto, Students>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.OwnerTeacherId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Class, opt => opt.Ignore())
                .ForMember(dest => dest.StudentResults, opt => opt.Ignore())
                .ForMember(dest => dest.AnswerSheets, opt => opt.Ignore());

            CreateMap<Students, StudentDto>()
                .ForMember(dest => dest.ClassName, opt => opt.MapFrom(src => src.Class != null ? src.Class.Name : ""));

            // Ánh xạ kết quả học tập
            CreateMap<CreateStudentResultDto, StudentResults>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Student, opt => opt.Ignore());

            CreateMap<StudentResults, StudentResultDto>()
                .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.Student != null ? src.Student.FullName : ""));

            // Ánh xạ Phiếu trả lời
            CreateMap<CreateAnswerSheetDto, AnswerSheets>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.OcrStatus, opt => opt.MapFrom(src => OcrStatus.PENDING))
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.ProcessedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Student, opt => opt.Ignore());

            CreateMap<AnswerSheets, AnswerSheetDto>()
                .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.Student != null ? src.Student.FullName : ""));
        }
    }
}