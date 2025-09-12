using FluentValidation;

namespace ClassroomService.Models.DTOs
{
    public class CreateClassDto
    {
        public string Name { get; set; }
        public int Grade { get; set; }
        public int HomeroomTeacherId { get; set; }
        public string AcademicYear { get; set; }
    }

    public class UpdateClassDto
    {
        public string Name { get; set; }
        public int Grade { get; set; }
        public string AcademicYear { get; set; }
        public bool IsActive { get; set; }
    }

    public class ClassDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Grade { get; set; }
        public int StudentCount { get; set; }
        public int HomeroomTeacherId { get; set; }
        public string AcademicYear { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CreateClassDtoValidator : AbstractValidator<CreateClassDto>
    {
        public CreateClassDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Tên lớp không được để trống")
                .MaximumLength(200).WithMessage("Tên lớp không được vượt quá 200 ký tự");
                
            RuleFor(x => x.Grade)
                .Must(x => x >= 10 && x <= 12)
                .WithMessage("Khối phải từ 10 đến 12");
                
            RuleFor(x => x.HomeroomTeacherId)
                .GreaterThan(0).WithMessage("ID giáo viên chủ nhiệm không hợp lệ");
                
            RuleFor(x => x.AcademicYear)
                .NotEmpty().WithMessage("Năm học không được để trống")
                .MaximumLength(50).WithMessage("Năm học không được vượt quá 50 ký tự");
        }
    }
}