using FluentValidation;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace OCRService.Models.Validators
{
    /// <summary>
    /// Validator cho file ảnh upload OCR
    /// </summary>
    public class OCRImageValidator : AbstractValidator<IFormFile>
    {
        public OCRImageValidator()
        {
            // Chỉ chấp nhận ảnh cho OCR
            RuleFor(x => x.ContentType)
                .Must(BeValidImageType)
                .WithMessage("Chỉ chấp nhận file ảnh: JPG, PNG, JPEG, HEIC, WebP");

            RuleFor(x => x.FileName)
                .Must(BeValidImageExtension)
                .WithMessage("File phải có đuôi: .jpg, .jpeg, .png, .heic, .webp");

            // Dung lượng ảnh - thực tế cho điện thoại
            RuleFor(x => x.Length)
                .LessThanOrEqualTo(25 * 1024 * 1024) // 25MB
                .WithMessage("Dung lượng ảnh không được vượt quá 25MB");

            RuleFor(x => x.Length)
                .GreaterThan(5 * 1024) // 5KB
                .WithMessage("Ảnh quá nhỏ, có thể bị lỗi OCR");

            // Kiểm tra file có rỗng không
            RuleFor(x => x.Length)
                .GreaterThan(0)
                .WithMessage("File không được rỗng");

            // Kiểm tra tên file
            RuleFor(x => x.FileName)
                .NotEmpty()
                .WithMessage("Tên file không được để trống");

            // Kiểm tra tên file không chứa ký tự đặc biệt
            RuleFor(x => x.FileName)
                .Must(BeValidFileName)
                .WithMessage("Tên file không được chứa ký tự đặc biệt");
        }

        /// <summary>
        /// Kiểm tra content type có hợp lệ không
        /// </summary>
        private bool BeValidImageType(string contentType)
        {
            if (string.IsNullOrEmpty(contentType))
                return false;

            var validTypes = new[] { 
                "image/jpeg", "image/jpg", "image/png", 
                "image/heic", "image/heif", "image/webp" 
            };
            
            return validTypes.Contains(contentType.ToLower());
        }

        /// <summary>
        /// Kiểm tra extension có hợp lệ không
        /// </summary>
        private bool BeValidImageExtension(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return false;

            var validExtensions = new[] { 
                ".jpg", ".jpeg", ".png", ".heic", ".heif", ".webp" 
            };
            
            var extension = Path.GetExtension(fileName).ToLower();
            return validExtensions.Contains(extension);
        }

        /// <summary>
        /// Kiểm tra tên file có hợp lệ không
        /// </summary>
        private bool BeValidFileName(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return false;

            // Không chứa ký tự đặc biệt
            var invalidChars = Path.GetInvalidFileNameChars();
            return !fileName.Any(c => invalidChars.Contains(c));
        }
    }

    /// <summary>
    /// Validator cho yêu cầu OCR
    /// </summary>
    public class YeuCauOCRValidator : AbstractValidator<DTOs.YeuCauOCRDto>
    {
        public YeuCauOCRValidator()
        {
            // Validate DeThiId
            RuleFor(x => x.DeThiId)
                .NotEmpty()
                .WithMessage("ID đề thi không được để trống");

            // Validate LoaiBaiLam
            RuleFor(x => x.LoaiBaiLam)
                .NotEmpty()
                .WithMessage("Loại bài làm không được để trống")
                .Must(BeValidLoaiBaiLam)
                .WithMessage("Loại bài làm không hợp lệ. Chỉ chấp nhận: TRAC_NGHIEM, TU_LUAN, DUNG_SAI");

            // Validate GiaoVienId
            RuleFor(x => x.GiaoVienId)
                .NotEmpty()
                .WithMessage("ID giáo viên không được để trống");

            // Validate AnhBaiLam
            RuleFor(x => x.AnhBaiLam)
                .NotNull()
                .WithMessage("File ảnh bài làm không được để trống");

            // Validate GhiChu
            RuleFor(x => x.GhiChu)
                .MaximumLength(500)
                .WithMessage("Ghi chú không được vượt quá 500 ký tự");

            // Validate ThoiGianTao
            RuleFor(x => x.ThoiGianTao)
                .LessThanOrEqualTo(DateTime.UtcNow.AddMinutes(5))
                .WithMessage("Thời gian tạo không hợp lệ");
        }

        /// <summary>
        /// Kiểm tra loại bài làm có hợp lệ không
        /// </summary>
        private bool BeValidLoaiBaiLam(string loaiBaiLam)
        {
            var validTypes = new[] { "TRAC_NGHIEM", "TU_LUAN", "DUNG_SAI" };
            return validTypes.Contains(loaiBaiLam);
        }
    }

    /// <summary>
    /// Validator cho kết quả OCR
    /// </summary>
    public class KetQuaOCRValidator : AbstractValidator<DTOs.KetQuaOCRDto>
    {
        public KetQuaOCRValidator()
        {
            // Validate OcrRequestId
            RuleFor(x => x.OcrRequestId)
                .NotEmpty()
                .WithMessage("ID yêu cầu OCR không được để trống");

            // Validate BaiLamId
            RuleFor(x => x.BaiLamId)
                .NotEmpty()
                .WithMessage("ID bài làm không được để trống");

            // Validate DeThiId
            RuleFor(x => x.DeThiId)
                .NotEmpty()
                .WithMessage("ID đề thi không được để trống");

            // Validate TrangThai
            RuleFor(x => x.TrangThai)
                .NotEmpty()
                .WithMessage("Trạng thái không được để trống")
                .Must(BeValidTrangThai)
                .WithMessage("Trạng thái không hợp lệ");

            // Validate DoChinhXac
            RuleFor(x => x.DoChinhXac)
                .InclusiveBetween(0.00m, 1.00m)
                .WithMessage("Độ chính xác phải từ 0.00 đến 1.00");

            // Validate ThoiGianXuLy
            RuleFor(x => x.ThoiGianXuLy)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Thời gian xử lý không được âm");

            // Validate AIProvider
            RuleFor(x => x.AIProvider)
                .NotEmpty()
                .WithMessage("AI provider không được để trống");

            // Validate DapAn
            RuleFor(x => x.DapAn)
                .NotEmpty()
                .WithMessage("Danh sách đáp án không được để trống");

            // Validate từng đáp án
            RuleForEach(x => x.DapAn)
                .SetValidator(new DapAnValidator());

            // Validate Diem (nếu có)
            When(x => x.Diem.HasValue, () =>
            {
                RuleFor(x => x.Diem!.Value)
                    .InclusiveBetween(0.00m, 10.00m)
                    .WithMessage("Điểm số phải từ 0.00 đến 10.00");
            });
        }

        /// <summary>
        /// Kiểm tra trạng thái có hợp lệ không
        /// </summary>
        private bool BeValidTrangThai(string trangThai)
        {
            var validStatuses = new[] { "PENDING", "PROCESSING", "COMPLETED", "FAILED", "FALLBACK_TO_MANUAL" };
            return validStatuses.Contains(trangThai);
        }
    }

    /// <summary>
    /// Validator cho đáp án từng câu hỏi
    /// </summary>
    public class DapAnValidator : AbstractValidator<DTOs.DapAnDto>
    {
        public DapAnValidator()
        {
            // Validate CauHoi
            RuleFor(x => x.CauHoi)
                .GreaterThan(0)
                .WithMessage("Số thứ tự câu hỏi phải lớn hơn 0");

            // Validate DapAn
            RuleFor(x => x.DapAn)
                .NotEmpty()
                .WithMessage("Đáp án không được để trống")
                .MaximumLength(10)
                .WithMessage("Đáp án không được vượt quá 10 ký tự");

            // Validate DoTinCay
            RuleFor(x => x.DoTinCay)
                .InclusiveBetween(0.00m, 1.00m)
                .WithMessage("Độ tin cậy phải từ 0.00 đến 1.00");

            // Validate Diem (nếu có)
            When(x => x.Diem.HasValue, () =>
            {
                RuleFor(x => x.Diem!.Value)
                    .GreaterThanOrEqualTo(0.00m)
                    .WithMessage("Điểm câu hỏi không được âm");
            });
        }
    }
}
