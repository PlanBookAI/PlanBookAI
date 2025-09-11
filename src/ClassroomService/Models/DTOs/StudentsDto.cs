using FluentValidation;
using ClassroomService.Models.Entities;

namespace ClassroomService.Models.DTOs
{
    public class CreateStudentDto
    {
        public string FullName { get; set; }
        public string StudentCode { get; set; }
        public DateOnly BirthDate { get; set; }
        public Gender Gender { get; set; }
        public int ClassId { get; set; }
        public int OwnerTeacherId { get; set; }
    }

    public class UpdateStudentDto
    {
        public string FullName { get; set; }
        public string StudentCode { get; set; }
        public DateOnly BirthDate { get; set; }
        public Gender Gender { get; set; }
        public int ClassId { get; set; }
        public bool IsActive { get; set; }
    }

    public class StudentDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string StudentCode { get; set; }
        public DateOnly BirthDate { get; set; }
        public Gender Gender { get; set; }
        public int ClassId { get; set; }
        public string ClassName { get; set; }
        public int OwnerTeacherId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CreateStudentDtoValidator : AbstractValidator<CreateStudentDto>
    {
        public CreateStudentDtoValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Họ và tên không được để trống")
                .MaximumLength(200).WithMessage("Họ và tên không được vượt quá 200 ký tự");
                
            RuleFor(x => x.StudentCode)
                .NotEmpty().WithMessage("Mã học sinh không được để trống")
                .MaximumLength(50).WithMessage("Mã học sinh không được vượt quá 50 ký tự");
                
            RuleFor(x => x.BirthDate)
                .NotEmpty().WithMessage("Ngày sinh không được để trống")
                .Must(BeValidAge).WithMessage("Ngày sinh không hợp lệ");
                
            RuleFor(x => x.ClassId)
                .GreaterThan(0).WithMessage("ID lớp học không hợp lệ");
                
            RuleFor(x => x.OwnerTeacherId)
                .GreaterThan(0).WithMessage("ID giáo viên không hợp lệ");
        }
        
        private bool BeValidAge(DateOnly birthDate)
        {
            var age = DateOnly.FromDateTime(DateTime.Today).Year - birthDate.Year;
            return age >= 14 && age <= 20;
        }
    }
}