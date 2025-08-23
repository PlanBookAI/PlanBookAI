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
                .MaximumLength(250).WithMessage("Tiêu đề không được vượt quá 250 ký tự.");

            RuleFor(x => x.MonHoc)
                .NotEmpty().WithMessage("Môn học không được để trống.");

            RuleFor(x => x.KhoiLop)
                .InclusiveBetween(10, 12).WithMessage("Khối lớp phải từ 10 đến 12.");

            RuleFor(x => x.CauTruc)
                .NotEmpty().WithMessage("Cấu trúc đề thi không được để trống.");

            RuleForEach(x => x.CauTruc).SetValidator(new YeuCauCauHoiDTOValidator());
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