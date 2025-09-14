using FluentValidation;
using ClassroomService.Models.Entities;

namespace ClassroomService.Models.DTOs
{
    /// <summary>
    /// DTO for creating student
    /// </summary>
    public class CreateStudentDto
    {
        /// <summary>
        /// Student full name
        /// </summary>
        public required string FullName { get; set; }
        
        /// <summary>
        /// Student code
        /// </summary>
        public required string StudentCode { get; set; }
        
        /// <summary>
        /// Birth date
        /// </summary>
        public DateOnly BirthDate { get; set; }
        
        /// <summary>
        /// Gender
        /// </summary>
        public Gender Gender { get; set; }
        
        /// <summary>
        /// Class ID
        /// </summary>
        public int ClassId { get; set; }
        
        /// <summary>
        /// Owner teacher ID
        /// </summary>
        public int OwnerTeacherId { get; set; }
    }

    /// <summary>
    /// DTO for updating student
    /// </summary>
    public class UpdateStudentDto
    {
        /// <summary>
        /// Student full name
        /// </summary>
        public required string FullName { get; set; }
        
        /// <summary>
        /// Student code
        /// </summary>
        public required string StudentCode { get; set; }
        
        /// <summary>
        /// Birth date
        /// </summary>
        public DateOnly BirthDate { get; set; }
        
        /// <summary>
        /// Gender
        /// </summary>
        public Gender Gender { get; set; }
        
        /// <summary>
        /// Class ID
        /// </summary>
        public int ClassId { get; set; }
        
        /// <summary>
        /// Active status
        /// </summary>
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// DTO for student response
    /// </summary>
    public class StudentDto
    {
        /// <summary>
        /// Student ID
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Student full name
        /// </summary>
        public required string FullName { get; set; }
        
        /// <summary>
        /// Student code
        /// </summary>
        public required string StudentCode { get; set; }
        
        /// <summary>
        /// Birth date
        /// </summary>
        public DateOnly BirthDate { get; set; }
        
        /// <summary>
        /// Gender
        /// </summary>
        public Gender Gender { get; set; }
        
        /// <summary>
        /// Class ID
        /// </summary>
        public int ClassId { get; set; }
        
        /// <summary>
        /// Class name
        /// </summary>
        public required string ClassName { get; set; }
        
        /// <summary>
        /// Owner teacher ID
        /// </summary>
        public int OwnerTeacherId { get; set; }
        
        /// <summary>
        /// Active status
        /// </summary>
        public bool IsActive { get; set; }
        
        /// <summary>
        /// Created timestamp
        /// </summary>
        public DateTime CreatedAt { get; set; }
        
        /// <summary>
        /// Updated timestamp
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }

    /// <summary>
    /// Validator for CreateStudentDto
    /// </summary>
    public class CreateStudentDtoValidator : AbstractValidator<CreateStudentDto>
    {
        /// <summary>
        /// Initializes validation rules
        /// </summary>
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