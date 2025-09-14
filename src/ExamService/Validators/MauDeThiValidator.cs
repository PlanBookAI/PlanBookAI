using ExamService.Models.DTOs;
using FluentValidation;

namespace ExamService.Validators
{
    public class MauDeThiRequestDTOValidator : AbstractValidator<MauDeThiRequestDTO>
    {
        public MauDeThiRequestDTOValidator()
        {
            RuleFor(x => x.TieuDe)
                .NotEmpty().WithMessage("Tiêu đề không được để trống.")
                .MaximumLength(255).WithMessage("Tiêu đề không được vượt quá 255 ký tự.");

            RuleFor(x => x.MonHoc)
                .NotEmpty().WithMessage("Môn học không được để trống.");

            RuleFor(x => x.KhoiLop)
                .InclusiveBetween(10, 12).WithMessage("Khối lớp phải từ 10 đến 12.")
                .When(x => x.KhoiLop.HasValue);

            RuleFor(x => x.ThoiGianLam)
                .InclusiveBetween(15, 300).WithMessage("Thời gian làm bài phải từ 15 đến 300 phút.")
                .When(x => x.ThoiGianLam.HasValue);

            RuleFor(x => x.TongDiem)
                .InclusiveBetween(1, 100).WithMessage("Tổng điểm phải từ 1 đến 100.")
                .When(x => x.TongDiem.HasValue);
        }
    }

    public class YeuCauCauHoiDTOValidator : AbstractValidator<YeuCauCauHoiDTO>
    {
        public YeuCauCauHoiDTOValidator()
        {
            RuleFor(x => x.ChuDe)
                .NotEmpty().WithMessage("Chủ đề của câu hỏi không được để trống.");

            RuleFor(x => x.DoKho)
                .NotEmpty().WithMessage("Độ khó của câu hỏi không được để trống.");

            RuleFor(x => x.SoLuong)
                .GreaterThan(0).WithMessage("Số lượng câu hỏi phải lớn hơn 0.");
        }
    }
}