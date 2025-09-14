using FluentValidation;
using ClassroomService.Models.Entities;

namespace ClassroomService.Models.DTOs
{
    /// <summary>
    /// Validator for UpdateStudentDto
    /// </summary>
    public class UpdateStudentDtoValidator : AbstractValidator<UpdateStudentDto>
    {
        /// <summary>
        /// Initializes validation rules for UpdateStudentDto
        /// </summary>
        public UpdateStudentDtoValidator()
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
        }
        
        private bool BeValidAge(DateOnly birthDate)
        {
            var age = DateOnly.FromDateTime(DateTime.Today).Year - birthDate.Year;
            return age >= 14 && age <= 20;
        }
    }

    /// <summary>
    /// Validator for UpdateClassDto
    /// </summary>
    public class UpdateClassDtoValidator : AbstractValidator<UpdateClassDto>
    {
        /// <summary>
        /// Initializes validation rules for UpdateClassDto
        /// </summary>
        public UpdateClassDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Tên lớp không được để trống")
                .MaximumLength(200).WithMessage("Tên lớp không được vượt quá 200 ký tự");
                
            RuleFor(x => x.Grade)
                .Must(x => x >= 10 && x <= 12)
                .WithMessage("Khối phải từ 10 đến 12");
                
            RuleFor(x => x.AcademicYear)
                .NotEmpty().WithMessage("Năm học không được để trống")
                .MaximumLength(50).WithMessage("Năm học không được vượt quá 50 ký tự");
        }
    }
}