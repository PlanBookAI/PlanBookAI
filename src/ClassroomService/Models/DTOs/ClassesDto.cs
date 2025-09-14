using FluentValidation;

namespace ClassroomService.Models.DTOs
{
    /// <summary>
    /// DTO for creating class
    /// </summary>
    public class CreateClassDto
    {
        /// <summary>
        /// Class name
        /// </summary>
        public required string Name { get; set; }
        
        /// <summary>
        /// Grade level
        /// </summary>
        public int Grade { get; set; }
        
        /// <summary>
        /// Homeroom teacher ID
        /// </summary>
        public int HomeroomTeacherId { get; set; }
        
        /// <summary>
        /// Academic year
        /// </summary>
        public required string AcademicYear { get; set; }
    }

    /// <summary>
    /// DTO for updating class
    /// </summary>
    public class UpdateClassDto
    {
        /// <summary>
        /// Class name
        /// </summary>
        public required string Name { get; set; }
        
        /// <summary>
        /// Grade level
        /// </summary>
        public int Grade { get; set; }
        
        /// <summary>
        /// Academic year
        /// </summary>
        public required string AcademicYear { get; set; }
        
        /// <summary>
        /// Active status
        /// </summary>
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// DTO for class response
    /// </summary>
    public class ClassDto
    {
        /// <summary>
        /// Class ID
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Class name
        /// </summary>
        public required string Name { get; set; }
        
        /// <summary>
        /// Grade level
        /// </summary>
        public int Grade { get; set; }
        
        /// <summary>
        /// Number of students
        /// </summary>
        public int StudentCount { get; set; }
        
        /// <summary>
        /// Homeroom teacher ID
        /// </summary>
        public int HomeroomTeacherId { get; set; }
        
        /// <summary>
        /// Academic year
        /// </summary>
        public required string AcademicYear { get; set; }
        
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
    /// Validator for CreateClassDto
    /// </summary>
    public class CreateClassDtoValidator : AbstractValidator<CreateClassDto>
    {
        /// <summary>
        /// Initializes validation rules
        /// </summary>
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