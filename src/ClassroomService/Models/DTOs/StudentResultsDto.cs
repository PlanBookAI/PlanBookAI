using FluentValidation;
using ClassroomService.Models.Entities;

namespace ClassroomService.Models.DTOs
{
    /// <summary>
    /// DTO for creating student result
    /// </summary>
    public class CreateStudentResultDto
    {
        /// <summary>
        /// Student ID
        /// </summary>
        public int StudentId { get; set; }
        
        /// <summary>
        /// Exam ID
        /// </summary>
        public int ExamId { get; set; }
        
        /// <summary>
        /// Score
        /// </summary>
        public decimal Score { get; set; }
        
        /// <summary>
        /// Actual duration in minutes
        /// </summary>
        public int ActualDuration { get; set; }
        
        /// <summary>
        /// Answer details in JSON format
        /// </summary>
        public required string AnswerDetails { get; set; }
        
        /// <summary>
        /// Grading method
        /// </summary>
        public GradingMethod GradingMethod { get; set; }
        
        /// <summary>
        /// Additional notes
        /// </summary>
        public required string Notes { get; set; }
        
        /// <summary>
        /// Exam date
        /// </summary>
        public DateTime ExamDate { get; set; }
    }

    /// <summary>
    /// DTO for student result response
    /// </summary>
    public class StudentResultDto
    {
        /// <summary>
        /// Result ID
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Student ID
        /// </summary>
        public int StudentId { get; set; }
        
        /// <summary>
        /// Student name
        /// </summary>
        public required string StudentName { get; set; }
        
        /// <summary>
        /// Exam ID
        /// </summary>
        public int ExamId { get; set; }
        
        /// <summary>
        /// Score
        /// </summary>
        public decimal Score { get; set; }
        
        /// <summary>
        /// Actual duration in minutes
        /// </summary>
        public int ActualDuration { get; set; }
        
        /// <summary>
        /// Answer details in JSON format
        /// </summary>
        public required string AnswerDetails { get; set; }
        
        /// <summary>
        /// Grading method
        /// </summary>
        public GradingMethod GradingMethod { get; set; }
        
        /// <summary>
        /// Additional notes
        /// </summary>
        public required string Notes { get; set; }
        
        /// <summary>
        /// Exam date
        /// </summary>
        public DateTime ExamDate { get; set; }
        
        /// <summary>
        /// Graded timestamp
        /// </summary>
        public DateTime? GradedAt { get; set; }
    }

    /// <summary>
    /// Validator for CreateStudentResultDto
    /// </summary>
    public class CreateStudentResultDtoValidator : AbstractValidator<CreateStudentResultDto>
    {
        /// <summary>
        /// Initializes validation rules
        /// </summary>
        public CreateStudentResultDtoValidator()
        {
            RuleFor(x => x.StudentId)
                .GreaterThan(0).WithMessage("ID học sinh không hợp lệ");

            RuleFor(x => x.ExamId)
                .GreaterThan(0).WithMessage("ID đề thi không hợp lệ");

            RuleFor(x => x.Score)
                .GreaterThanOrEqualTo(0).WithMessage("Điểm số không được âm")
                .LessThanOrEqualTo(10).WithMessage("Điểm số không được vượt quá 10");

            RuleFor(x => x.ActualDuration)
                .GreaterThan(0).WithMessage("Thời gian làm bài phải lớn hơn 0");

            RuleFor(x => x.ExamDate)
                .NotEmpty().WithMessage("Ngày thi không được để trống");
        }
    }
}