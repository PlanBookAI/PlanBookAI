using PlanService.Models.DTOs;
using PlanService.Models.Entities;
using PlanService.Repositories;
using System.Text.Json;
// using PlanService.Services.Export;

namespace PlanService.Services
{
    /// <summary>
    /// Implementation của IGiaoAnService
    /// Xử lý business logic cho Giáo Án theo yêu cầu ban đầu
    /// </summary>
    public class GiaoAnService : IGiaoAnService
    {
        private readonly IGiaoAnRepository _giaoAnRepository;
        // private readonly IXuatGiaoAnPdfService _xuatPdf;
        // private readonly IXuatGiaoAnWordService _xuatWord;

        public GiaoAnService(IGiaoAnRepository giaoAnRepository /*, IXuatGiaoAnWordService xuatWord, IXuatGiaoAnPdfService xuatPdf */)
        {
            _giaoAnRepository = giaoAnRepository;
            // _xuatWord = xuatWord;
            // _xuatPdf = xuatPdf;
        }

        // CRUD Operations
        public async Task<ApiPhanHoi<IEnumerable<GiaoAn>>> GetAllAsync(Guid teacherId)
        {
            try
            {
                var giaoAns = await _giaoAnRepository.GetByTeacherIdAsync(teacherId);
                return ApiPhanHoi<IEnumerable<GiaoAn>>.ThanhCongOk(giaoAns, "Lấy danh sách giáo án thành công");
            }
            catch (Exception ex)
            {
                return ApiPhanHoi<IEnumerable<GiaoAn>>.ThatBai("Lỗi khi lấy danh sách giáo án: " + ex.Message);
            }
        }

        public async Task<ApiPhanHoi<GiaoAn?>> GetByIdAsync(Guid id, Guid teacherId)
        {
            try
            {
                var giaoAn = await _giaoAnRepository.GetByIdAndTeacherIdAsync(id, teacherId);
                if (giaoAn == null)
                {
                    return ApiPhanHoi<GiaoAn?>.ThatBai("Không tìm thấy giáo án");
                }
                return ApiPhanHoi<GiaoAn?>.ThanhCongOk(giaoAn, "Lấy chi tiết giáo án thành công");
            }
            catch (Exception ex)
            {
                return ApiPhanHoi<GiaoAn?>.ThatBai("Lỗi khi lấy chi tiết giáo án: " + ex.Message);
            }
        }

        public async Task<ApiPhanHoi<GiaoAn>> CreateAsync(YeuCauTaoGiaoAn request, Guid teacherId)
        {
            try
            {
                var giaoAn = new GiaoAn
                {
                    Id = Guid.NewGuid(),
                    TieuDe = request.TieuDe,
                    MucTieu = request.MucTieu,
                    NoiDung = new Dictionary<string, object>
                    {
                        ["ThoiLuongTiet"] = request.ThoiLuongTiet,
                        ["LopHoc"] = request.LopHoc ?? "",
                        ["GhiChu"] = request.GhiChu ?? "",
                        ["SuDungAI"] = request.SuDungAI,
                        ["YeuCauDacBiet"] = request.YeuCauDacBiet ?? ""
                    },
                    MonHoc = request.MonHoc.ToString(),
                    Lop = request.Khoi,
                    GiaoVienId = teacherId,
                    MauGiaoAnId = request.MauGiaoAnId,
                    ChuDeId = request.ChuDeId,
                    TrangThai = "DRAFT",
                    TaoLuc = DateTime.UtcNow,
                    CapNhatLuc = DateTime.UtcNow
                };

                await _giaoAnRepository.AddAsync(giaoAn);
                return ApiPhanHoi<GiaoAn>.ThanhCongOk(giaoAn, "Tạo giáo án thành công");
            }
            catch (Exception ex)
            {
                var errorDetails = ex.Message;
                if (ex.InnerException != null)
                {
                    errorDetails += " | Inner: " + ex.InnerException.Message;
                    if (ex.InnerException.InnerException != null)
                    {
                        errorDetails += " | Inner2: " + ex.InnerException.InnerException.Message;
                    }
                }
                return ApiPhanHoi<GiaoAn>.ThatBai("Lỗi khi tạo giáo án: " + errorDetails);
            }
        }

        public async Task<ApiPhanHoi<GiaoAn>> UpdateAsync(Guid id, YeuCauTaoGiaoAn request, Guid teacherId)
        {
            try
            {
                var giaoAn = await _giaoAnRepository.GetByIdAndTeacherIdAsync(id, teacherId);
                if (giaoAn == null)
                {
                    return ApiPhanHoi<GiaoAn>.ThatBai("Không tìm thấy giáo án");
                }

                giaoAn.TieuDe = request.TieuDe;
                giaoAn.MucTieu = request.MucTieu;
                giaoAn.MonHoc = request.MonHoc.ToString();
                giaoAn.Lop = request.Khoi;
                giaoAn.MauGiaoAnId = request.MauGiaoAnId;
                giaoAn.ChuDeId = request.ChuDeId;
                giaoAn.NoiDung = new Dictionary<string, object>
                {
                    ["ThoiLuongTiet"] = request.ThoiLuongTiet,
                    ["LopHoc"] = request.LopHoc ?? "",
                    ["GhiChu"] = request.GhiChu ?? "",
                    ["SuDungAI"] = request.SuDungAI,
                    ["YeuCauDacBiet"] = request.YeuCauDacBiet ?? ""
                };
                giaoAn.CapNhatLuc = DateTime.UtcNow;

                await _giaoAnRepository.UpdateAsync(giaoAn);
                return ApiPhanHoi<GiaoAn>.ThanhCongOk(giaoAn, "Cập nhật giáo án thành công");
            }
            catch (Exception ex)
            {
                return ApiPhanHoi<GiaoAn>.ThatBai("Lỗi khi cập nhật giáo án: " + ex.Message);
            }
        }

        public async Task<ApiPhanHoi<GiaoAn>> DeleteAsync(Guid id, Guid teacherId)
        {
            try
            {
                var giaoAn = await _giaoAnRepository.GetByIdAndTeacherIdAsync(id, teacherId);
                if (giaoAn == null)
                {
                    return ApiPhanHoi<GiaoAn>.ThatBai("Không tìm thấy giáo án");
                }

                await _giaoAnRepository.DeleteAsync(id);
                return ApiPhanHoi<GiaoAn>.ThanhCongOk(giaoAn, "Xóa giáo án thành công");
            }
            catch (Exception ex)
            {
                return ApiPhanHoi<GiaoAn>.ThatBai("Lỗi khi xóa giáo án: " + ex.Message);
            }
        }

        // Status Management (theo yêu cầu: Draft → Completed → Published → Archived)
        public async Task<ApiPhanHoi<GiaoAn>> PheDuyetAsync(Guid id, Guid teacherId)
        {
            try
            {
                var giaoAn = await _giaoAnRepository.UpdateStatusAsync(id, "COMPLETED");
                if (giaoAn == null)
                {
                    return ApiPhanHoi<GiaoAn>.ThatBai("Không tìm thấy giáo án");
                }
                return ApiPhanHoi<GiaoAn>.ThanhCongOk(giaoAn, "Phê duyệt giáo án thành công");
            }
            catch (Exception ex)
            {
                return ApiPhanHoi<GiaoAn>.ThatBai("Lỗi khi phê duyệt giáo án: " + ex.Message);
            }
        }

        public async Task<ApiPhanHoi<GiaoAn>> XuatBanAsync(Guid id, Guid teacherId)
        {
            try
            {
                var giaoAn = await _giaoAnRepository.UpdateStatusAsync(id, "PUBLISHED");
                if (giaoAn == null)
                {
                    return ApiPhanHoi<GiaoAn>.ThatBai("Không tìm thấy giáo án");
                }
                return ApiPhanHoi<GiaoAn>.ThanhCongOk(giaoAn, "Xuất bản giáo án thành công");
            }
            catch (Exception ex)
            {
                return ApiPhanHoi<GiaoAn>.ThatBai("Lỗi khi xuất bản giáo án: " + ex.Message);
            }
        }

        public async Task<ApiPhanHoi<GiaoAn>> LuuTruAsync(Guid id, Guid teacherId)
        {
            try
            {
                var giaoAn = await _giaoAnRepository.UpdateStatusAsync(id, "ARCHIVED");
                if (giaoAn == null)
                {
                    return ApiPhanHoi<GiaoAn>.ThatBai("Không tìm thấy giáo án");
                }
                return ApiPhanHoi<GiaoAn>.ThanhCongOk(giaoAn, "Lưu trữ giáo án thành công");
            }
            catch (Exception ex)
            {
                return ApiPhanHoi<GiaoAn>.ThatBai("Lỗi khi lưu trữ giáo án: " + ex.Message);
            }
        }

        public async Task<ApiPhanHoi<GiaoAn>> CopyAsync(Guid id, Guid teacherId)
        {
            try
            {
                var giaoAnGoc = await _giaoAnRepository.GetByIdAndTeacherIdAsync(id, teacherId);
                if (giaoAnGoc == null)
                {
                    return ApiPhanHoi<GiaoAn>.ThatBai("Không tìm thấy giáo án");
                }

                var giaoAnMoi = new GiaoAn
                {
                    Id = Guid.NewGuid(),
                    TieuDe = giaoAnGoc.TieuDe + " (Bản sao)",
                    MucTieu = giaoAnGoc.MucTieu,
                    NoiDung = giaoAnGoc.NoiDung,
                    MonHoc = giaoAnGoc.MonHoc,
                    Lop = giaoAnGoc.Lop,
                    GiaoVienId = teacherId,
                    MauGiaoAnId = giaoAnGoc.MauGiaoAnId,
                    TrangThai = "DRAFT",
                    TaoLuc = DateTime.UtcNow,
                    CapNhatLuc = DateTime.UtcNow
                };

                await _giaoAnRepository.AddAsync(giaoAnMoi);
                return ApiPhanHoi<GiaoAn>.ThanhCongOk(giaoAnMoi, "Sao chép giáo án thành công");
            }
            catch (Exception ex)
            {
                return ApiPhanHoi<GiaoAn>.ThatBai("Lỗi khi sao chép giáo án: " + ex.Message);
            }
        }

        // Search & Filter (theo yêu cầu API endpoints)
        public async Task<ApiPhanHoi<IEnumerable<GiaoAn>>> TimKiemAsync(string keyword, Guid teacherId)
        {
            try
            {
                var giaoAns = await _giaoAnRepository.SearchAsync(keyword, teacherId);
                return ApiPhanHoi<IEnumerable<GiaoAn>>.ThanhCongOk(giaoAns, "Tìm kiếm giáo án thành công");
            }
            catch (Exception ex)
            {
                return ApiPhanHoi<IEnumerable<GiaoAn>>.ThatBai("Lỗi khi tìm kiếm giáo án: " + ex.Message);
            }
        }

        public async Task<ApiPhanHoi<IEnumerable<GiaoAn>>> LocTheoChuDeAsync(Guid chuDeId, Guid teacherId)
        {
            try
            {
                // Lọc giáo án theo chủ đề của giáo viên
                var giaoAns = await _giaoAnRepository.GetByTeacherIdAndTopicAsync(teacherId, chuDeId);
                return ApiPhanHoi<IEnumerable<GiaoAn>>.ThanhCongOk(giaoAns, "Lọc theo chủ đề thành công");
            }
            catch (Exception ex)
            {
                return ApiPhanHoi<IEnumerable<GiaoAn>>.ThatBai("Lỗi khi lọc theo chủ đề: " + ex.Message);
            }
        }

        public async Task<ApiPhanHoi<IEnumerable<GiaoAn>>> LocTheoMonHocAsync(string monHoc, Guid teacherId)
        {
            try
            {
                var giaoAns = await _giaoAnRepository.GetByTeacherIdAndSubjectAsync(teacherId, monHoc);
                return ApiPhanHoi<IEnumerable<GiaoAn>>.ThanhCongOk(giaoAns, "Lọc theo môn học thành công");
            }
            catch (Exception ex)
            {
                return ApiPhanHoi<IEnumerable<GiaoAn>>.ThatBai("Lỗi khi lọc theo môn học: " + ex.Message);
            }
        }

        public async Task<ApiPhanHoi<IEnumerable<GiaoAn>>> LocTheoKhoiAsync(int khoi, Guid teacherId)
        {
            try
            {
                var giaoAns = await _giaoAnRepository.GetByTeacherIdAndGradeAsync(teacherId, khoi);
                return ApiPhanHoi<IEnumerable<GiaoAn>>.ThanhCongOk(giaoAns, "Lọc theo khối lớp thành công");
            }
            catch (Exception ex)
            {
                return ApiPhanHoi<IEnumerable<GiaoAn>>.ThatBai("Lỗi khi lọc theo khối lớp: " + ex.Message);
            }
        }

        // Export (theo yêu cầu: Word/PDF với định dạng chuẩn giáo dục)
        public async Task<ApiPhanHoi<byte[]>> XuatPDFAsync(Guid id, Guid teacherId)
        {
            #if false
            return await _xuatPdf.XuatPdfAsync(id, teacherId);
            #endif
            return ApiPhanHoi<byte[]>.ThatBai("Tính năng xuất PDF tạm thời vô hiệu hóa.");
        }

        public async Task<ApiPhanHoi<byte[]>> XuatWordAsync(Guid id, Guid teacherId)
        {
            #if false
            return await _xuatWord.XuatWordAsync(id, teacherId);
            #endif
            return ApiPhanHoi<byte[]>.ThatBai("Tính năng xuất Word tạm thời vô hiệu hóa. Vui lòng thử lại sau.");
        }

        // Create from Template (theo yêu cầu: Tạo giáo án từ mẫu có sẵn)
        public async Task<ApiPhanHoi<GiaoAn>> TaoTuMauAsync(Guid templateId, YeuCauTaoGiaoAn request, Guid teacherId)
        {
            try
            {
                // TODO: Implement khi có MauGiaoAn relationship
                var giaoAn = new GiaoAn
                {
                    Id = Guid.NewGuid(),
                    TieuDe = request.TieuDe,
                    MucTieu = request.MucTieu,
                    NoiDung = new Dictionary<string, object>(),
                    MonHoc = request.MonHoc.ToString(),
                    Lop = request.Khoi,
                    GiaoVienId = teacherId,
                    MauGiaoAnId = templateId,
                    TrangThai = "DRAFT",
                    TaoLuc = DateTime.UtcNow,
                    CapNhatLuc = DateTime.UtcNow
                };

                await _giaoAnRepository.AddAsync(giaoAn);
                return ApiPhanHoi<GiaoAn>.ThanhCongOk(giaoAn, "Tạo giáo án từ mẫu thành công");
            }
            catch (Exception ex)
            {
                return ApiPhanHoi<GiaoAn>.ThatBai("Lỗi khi tạo giáo án từ mẫu: " + ex.Message);
            }
        }
    }
}