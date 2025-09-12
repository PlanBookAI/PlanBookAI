using FluentValidation;
using ClassroomService.Models.Entities;

namespace ClassroomService.Models.DTOs
{
    public class CreateStudentResultDto
    {
        public int StudentId { get; set; }
        public int ExamId { get; set; }
        public decimal Score { get; set; }
        public int ActualDuration { get; set; }
        public string AnswerDetails { get; set; }
        public GradingMethod GradingMethod { get; set; }
        public string Notes { get; set; }
        public DateTime ExamDate { get; set; }
    }

    public class StudentResultDto
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public int ExamId { get; set; }
        public decimal Score { get; set; }
        public int ActualDuration { get; set; }
        public string AnswerDetails { get; set; }
        public GradingMethod GradingMethod { get; set; }
        public string Notes { get; set; }
        public DateTime ExamDate { get; set; }
        public DateTime? GradedAt { get; set; }
    }

    public class CreateStudentResultDtoValidator : AbstractValidator<CreateStudentResultDto>
    {
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