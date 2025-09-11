using FluentValidation;
using ClassroomService.Models.Entities;

namespace ClassroomService.Models.DTOs
{
    public class CreateAnswerSheetDto
    {
        public int StudentId { get; set; }
        public int ExamId { get; set; }
        public string ImageUrl { get; set; }
    }

    public class AnswerSheetDto
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public int ExamId { get; set; }
        public string ImageUrl { get; set; }
        public string OcrResult { get; set; }
        public OcrStatus OcrStatus { get; set; }
        public decimal? OcrAccuracy { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateAnswerSheetDtoValidator : AbstractValidator<CreateAnswerSheetDto>
    {
        public CreateAnswerSheetDtoValidator()
        {
            RuleFor(x => x.StudentId)
                .GreaterThan(0).WithMessage("ID học sinh không hợp lệ");

            RuleFor(x => x.ExamId)
                .GreaterThan(0).WithMessage("ID đề thi không hợp lệ");

            RuleFor(x => x.ImageUrl)
                .NotEmpty().WithMessage("URL hình ảnh không được để trống")
                .MaximumLength(500).WithMessage("URL hình ảnh không được vượt quá 500 ký tự");
        }
    }
}