using FluentValidation;
using UserService.Models.DTOs;

namespace UserService.Models.Validators;

public class YeuCauCapNhatHoSoValidator : AbstractValidator<YeuCauCapNhatHoSo>
{
    public YeuCauCapNhatHoSoValidator()
    {
        RuleFor(x => x.HoTen)
            .NotEmpty().WithMessage("Họ tên không được để trống")
            .MaximumLength(255).WithMessage("Họ tên không được quá 255 ký tự");

        RuleFor(x => x.SoDienThoai)
            .MaximumLength(20).WithMessage("Số điện thoại không được quá 20 ký tự")
            .Matches(@"^[0-9+\-\s()]*$").When(x => !string.IsNullOrEmpty(x.SoDienThoai))
            .WithMessage("Số điện thoại không hợp lệ");

        RuleFor(x => x.DiaChi)
            .MaximumLength(500).WithMessage("Địa chỉ không được quá 500 ký tự");

        RuleFor(x => x.MoTaBanThan)
            .MaximumLength(1000).WithMessage("Mô tả bản thân không được quá 1000 ký tự");

        RuleFor(x => x.AnhDaiDienUrl)
            .MaximumLength(500).WithMessage("URL ảnh đại diện không được quá 500 ký tự")
            .Must(BeValidUrl).When(x => !string.IsNullOrEmpty(x.AnhDaiDienUrl))
            .WithMessage("URL ảnh đại diện không hợp lệ");
    }

    private bool BeValidUrl(string? url)
    {
        if (string.IsNullOrEmpty(url)) return true;
        return Uri.TryCreate(url, UriKind.Absolute, out _);
    }
}
