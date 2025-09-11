using PlanService.Models.DTOs;
using PlanService.Models.Entities;
using PlanService.Repositories;
using System.Text.Json;

namespace PlanService.Services
{
    /// <summary>
    /// Implementation của IGiaoAnService
    /// Xử lý business logic cho Giáo Án theo yêu cầu ban đầu
    /// </summary>
    public class GiaoAnService : IGiaoAnService
    {
        private readonly IGiaoAnRepository _giaoAnRepository;
        private readonly IMauGiaoAnRepository _mauGiaoAnRepository;

        public GiaoAnService(IGiaoAnRepository giaoAnRepository, IMauGiaoAnRepository mauGiaoAnRepository)
        {
            _giaoAnRepository = giaoAnRepository;
            _mauGiaoAnRepository = mauGiaoAnRepository;
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

        public async Task<ApiPhanHoi<ThongTinGiaoAn?>> GetByIdAsync(Guid id, Guid teacherId)
        {
            try
            {
                var giaoAn = await _giaoAnRepository.GetByIdAndTeacherIdAsync(id, teacherId);
                if (giaoAn == null)
                {
                    return ApiPhanHoi<ThongTinGiaoAn?>.ThatBai("Không tìm thấy giáo án");
                }

                var thongTin = new ThongTinGiaoAn
                {
                    Id = giaoAn.Id,
                    TieuDe = giaoAn.TieuDe,
                    MucTieu = giaoAn.MucTieu,
                    NoiDung = giaoAn.NoiDung,
                    MonHoc = giaoAn.MonHoc,
                    TrangThai = giaoAn.TrangThai,
                    NgayTao = giaoAn.TaoLuc,
                    NgayCapNhat = giaoAn.CapNhatLuc,
                    NguoiTaoId = giaoAn.GiaoVienId,
                    MauGiaoAnId = giaoAn.MauGiaoAnId,
                    // Extract an toàn từ NoiDung dictionary
                    ThoiLuongTiet = GetIntFromDictionary(giaoAn.NoiDung, "ThoiLuongTiet", 1),
                    LopHoc = GetStringFromDictionary(giaoAn.NoiDung, "LopHoc"),
                    GhiChu = GetStringFromDictionary(giaoAn.NoiDung, "GhiChu"),
                    YeuCauDacBiet = GetStringFromDictionary(giaoAn.NoiDung, "YeuCauDacBiet"),
                    DuocTaoTuAI = GetBoolFromDictionary(giaoAn.NoiDung, "SuDungAI", false)
                };

                return ApiPhanHoi<ThongTinGiaoAn?>.ThanhCongOk(thongTin, "Lấy chi tiết giáo án thành công");
            }
            catch (Exception ex)
            {
                return ApiPhanHoi<ThongTinGiaoAn?>.ThatBai("Lỗi khi lấy chi tiết giáo án: " + ex.Message);
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
                    NoiDung = BuildNoiDungFromRequest(request),
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
                giaoAn.NoiDung = BuildNoiDungFromRequest(request);
                // Chuẩn hóa thời gian về UTC tránh lỗi timestamptz
                if (giaoAn.TaoLuc.Kind == DateTimeKind.Unspecified)
                {
                    giaoAn.TaoLuc = DateTime.SpecifyKind(giaoAn.TaoLuc, DateTimeKind.Utc);
                }
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

        // Export (tạm thời vô hiệu hóa - sẽ implement trong tương lai)
        public async Task<ApiPhanHoi<byte[]>> XuatPDFAsync(Guid id, Guid teacherId)
        {
            await Task.Delay(1); // Placeholder để tránh warning
            return ApiPhanHoi<byte[]>.ThatBai("Tính năng xuất PDF chưa được triển khai.");
        }

        public async Task<ApiPhanHoi<byte[]>> XuatWordAsync(Guid id, Guid teacherId)
        {
            await Task.Delay(1); // Placeholder để tránh warning
            return ApiPhanHoi<byte[]>.ThatBai("Tính năng xuất Word chưa được triển khai.");
        }

        // Create from Template (theo yêu cầu: Tạo giáo án từ mẫu có sẵn)
        public async Task<ApiPhanHoi<GiaoAn>> TaoTuMauAsync(Guid templateId, YeuCauTaoGiaoAn request, Guid teacherId)
        {
            try
            {
                // Lấy mẫu và kiểm tra quyền truy cập (công khai hoặc là chủ sở hữu)
                var mau = await _mauGiaoAnRepository.GetByIdAsync(templateId);
                if (mau == null)
                {
                    return ApiPhanHoi<GiaoAn>.ThatBai("Không tìm thấy mẫu giáo án");
                }

                var laChuSoHuu = mau.NguoiTaoId == teacherId;
                var laCongKhai = string.Equals(mau.TrangThai, "ACTIVE", StringComparison.OrdinalIgnoreCase);
                if (!laChuSoHuu && !laCongKhai)
                {
                    return ApiPhanHoi<GiaoAn>.ThatBai("Bạn không có quyền sử dụng mẫu này");
                }

                // Snapshot toàn bộ nội dung mẫu sang content của giáo án mới
                var snapshotNoiDung = DeepCloneDictionary(mau.NoiDungMau);

                // Ghép thêm phần ThongTinChung từ request
                snapshotNoiDung["ThoiLuongTiet"] = request.ThoiLuongTiet;
                snapshotNoiDung["LopHoc"] = request.LopHoc ?? string.Empty;
                snapshotNoiDung["GhiChu"] = request.GhiChu ?? string.Empty;
                snapshotNoiDung["SuDungAI"] = request.SuDungAI;
                snapshotNoiDung["YeuCauDacBiet"] = request.YeuCauDacBiet ?? string.Empty;

                var giaoAn = new GiaoAn
                {
                    Id = Guid.NewGuid(),
                    TieuDe = request.TieuDe,
                    MucTieu = request.MucTieu,
                    NoiDung = snapshotNoiDung,
                    MonHoc = request.MonHoc.ToString(),
                    Lop = request.Khoi,
                    GiaoVienId = teacherId,
                    MauGiaoAnId = templateId,
                    ChuDeId = request.ChuDeId,
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

        private static Dictionary<string, object> DeepCloneDictionary(Dictionary<string, object> source)
        {
            // Clone an toàn qua (de)serialize để tránh tham chiếu chung
            var json = System.Text.Json.JsonSerializer.Serialize(source);
            return System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(json) ?? new Dictionary<string, object>();
        }

        private static Dictionary<string, object> BuildNoiDungFromRequest(YeuCauTaoGiaoAn request)
        {
            var noiDung = new Dictionary<string, object>
            {
                ["ThoiLuongTiet"] = request.ThoiLuongTiet,
                ["LopHoc"] = request.LopHoc ?? string.Empty,
                ["GhiChu"] = request.GhiChu ?? string.Empty,
                ["SuDungAI"] = request.SuDungAI,
                ["YeuCauDacBiet"] = request.YeuCauDacBiet ?? string.Empty
            };

            if (request.NoiDungChiTiet != null)
            {
                // Lưu toàn bộ nội dung chi tiết vào key 'NoiDungChiTiet'
                noiDung["NoiDungChiTiet"] = request.NoiDungChiTiet;
            }

            return noiDung;
        }

        // Helper methods để xử lý JsonElement an toàn
        private static T GetValueFromDictionary<T>(Dictionary<string, object> dict, string key, T defaultValue)
        {
            if (!dict.ContainsKey(key) || dict[key] == null)
                return defaultValue;

            var value = dict[key];

            // Handle JsonElement
            if (value is System.Text.Json.JsonElement jsonElement)
            {
                try
                {
                    return typeof(T) switch
                    {
                        var t when t == typeof(int) => (T)(object)jsonElement.GetInt32(),
                        var t when t == typeof(bool) => (T)(object)jsonElement.GetBoolean(),
                        var t when t == typeof(string) => (T)(object)(jsonElement.GetString() ?? string.Empty),
                        _ => defaultValue
                    };
                }
                catch
                {
                    return defaultValue;
                }
            }

            // Handle direct conversion
            try
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch
            {
                return defaultValue;
            }
        }

        private static string? GetStringFromDictionary(Dictionary<string, object> dict, string key)
        {
            return GetValueFromDictionary<string?>(dict, key, null);
        }

        private static int GetIntFromDictionary(Dictionary<string, object> dict, string key, int defaultValue = 0)
        {
            return GetValueFromDictionary(dict, key, defaultValue);
        }

        private static bool GetBoolFromDictionary(Dictionary<string, object> dict, string key, bool defaultValue = false)
        {
            return GetValueFromDictionary(dict, key, defaultValue);
        }
    }
}