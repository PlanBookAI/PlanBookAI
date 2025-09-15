using FluentValidation;
using ClassroomService.Models.Entities;

namespace ClassroomService.Models.DTOs
{
    /// <summary>
    /// DTO for creating answer sheet
    /// </summary>
    public class CreateAnswerSheetDto
    {
        /// <summary>
        /// Student ID
        /// </summary>
        public Guid StudentId { get; set; }
        
        /// <summary>
        /// Exam ID
        /// </summary>
        public Guid ExamId { get; set; }
        
        /// <summary>
        /// Image URL
        /// </summary>
        public required string ImageUrl { get; set; }
    }

    /// <summary>
    /// DTO for answer sheet response
    /// </summary>
    public class AnswerSheetDto
    {
        /// <summary>
        /// Answer sheet ID
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// Student ID
        /// </summary>
        public Guid StudentId { get; set; }
        
        /// <summary>
        /// Student name
        /// </summary>
        public required string StudentName { get; set; }
        
        /// <summary>
        /// Exam ID
        /// </summary>
        public Guid ExamId { get; set; }
        
        /// <summary>
        /// Image URL
        /// </summary>
        public required string ImageUrl { get; set; }
        
        /// <summary>
        /// OCR result
        /// </summary>
        public required string OcrResult { get; set; }
        
        /// <summary>
        /// OCR status
        /// </summary>
        public OcrStatus OcrStatus { get; set; }
        
        /// <summary>
        /// OCR accuracy percentage
        /// </summary>
        public decimal? OcrAccuracy { get; set; }
        
        /// <summary>
        /// Processed timestamp
        /// </summary>
        public DateTime? ProcessedAt { get; set; }
        
        /// <summary>
        /// Created timestamp
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// Validator for CreateAnswerSheetDto
    /// </summary>
    public class CreateAnswerSheetDtoValidator : AbstractValidator<CreateAnswerSheetDto>
    {
        /// <summary>
        /// Initializes validation rules
        /// </summary>
        public CreateAnswerSheetDtoValidator()
        {
            RuleFor(x => x.StudentId)
                .NotEmpty().WithMessage("ID học sinh không được để trống");

            RuleFor(x => x.ExamId)
                .NotEmpty().WithMessage("ID đề thi không được để trống");

            RuleFor(x => x.ImageUrl)
                .NotEmpty().WithMessage("URL hình ảnh không được để trống")
                .MaximumLength(500).WithMessage("URL hình ảnh không được vượt quá 500 ký tự");
        }
    }
}