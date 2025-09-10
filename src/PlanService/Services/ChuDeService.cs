using PlanService.Models.DTOs;
using PlanService.Models.Entities;
using PlanService.Repositories;

namespace PlanService.Services
{
    /// <summary>
    /// Service xử lý business logic cho quản lý chủ đề giáo dục.
    /// </summary>
    public class ChuDeService : IChuDeService
    {
        private readonly IChuDeRepository _chuDeRepository;

        public ChuDeService(IChuDeRepository chuDeRepository)
        {
            _chuDeRepository = chuDeRepository;
        }

        public async Task<ApiPhanHoi<IEnumerable<ChuDe>>> GetAllAsync()
        {
            try
            {
                var chuDes = await _chuDeRepository.GetAllAsync();
                return ApiPhanHoi<IEnumerable<ChuDe>>.ThanhCongOk(chuDes, "Lấy danh sách chủ đề thành công");
            }
            catch (Exception ex)
            {
                return ApiPhanHoi<IEnumerable<ChuDe>>.ThatBai("Lỗi khi lấy danh sách chủ đề: " + ex.Message);
            }
        }

        public async Task<ApiPhanHoi<ChuDe?>> GetByIdAsync(Guid id)
        {
            try
            {
                var chuDe = await _chuDeRepository.GetByIdAsync(id);
                if (chuDe == null)
                {
                    return ApiPhanHoi<ChuDe?>.ThatBai("Không tìm thấy chủ đề với ID: " + id);
                }
                return ApiPhanHoi<ChuDe?>.ThanhCongOk(chuDe, "Lấy chủ đề thành công");
            }
            catch (Exception ex)
            {
                return ApiPhanHoi<ChuDe?>.ThatBai("Lỗi khi lấy chủ đề: " + ex.Message);
            }
        }

        public async Task<ApiPhanHoi<IEnumerable<ChuDe>>> GetByMonHocAsync(string monHoc)
        {
            try
            {
                var chuDes = await _chuDeRepository.GetByMonHocAsync(monHoc);
                return ApiPhanHoi<IEnumerable<ChuDe>>.ThanhCongOk(chuDes, "Lấy chủ đề theo môn học thành công");
            }
            catch (Exception ex)
            {
                return ApiPhanHoi<IEnumerable<ChuDe>>.ThatBai("Lỗi khi lấy chủ đề theo môn học: " + ex.Message);
            }
        }

        public async Task<ApiPhanHoi<IEnumerable<ChuDe>>> GetByParentIdAsync(Guid? parentId)
        {
            try
            {
                var chuDes = await _chuDeRepository.GetByParentIdAsync(parentId);
                return ApiPhanHoi<IEnumerable<ChuDe>>.ThanhCongOk(chuDes, "Lấy chủ đề con thành công");
            }
            catch (Exception ex)
            {
                return ApiPhanHoi<IEnumerable<ChuDe>>.ThatBai("Lỗi khi lấy chủ đề con: " + ex.Message);
            }
        }

        public async Task<ApiPhanHoi<ChuDe>> AddAsync(ChuDe chuDe)
        {
            try
            {
                await _chuDeRepository.AddAsync(chuDe);
                return ApiPhanHoi<ChuDe>.ThanhCongOk(chuDe, "Tạo chủ đề thành công");
            }
            catch (Exception ex)
            {
                return ApiPhanHoi<ChuDe>.ThatBai("Lỗi khi tạo chủ đề: " + ex.Message);
            }
        }

        public async Task<ApiPhanHoi<ChuDe>> UpdateAsync(ChuDe chuDe)
        {
            try
            {
                await _chuDeRepository.UpdateAsync(chuDe);
                return ApiPhanHoi<ChuDe>.ThanhCongOk(chuDe, "Cập nhật chủ đề thành công");
            }
            catch (Exception ex)
            {
                return ApiPhanHoi<ChuDe>.ThatBai("Lỗi khi cập nhật chủ đề: " + ex.Message);
            }
        }

        public async Task<ApiPhanHoi<ChuDe>> DeleteAsync(Guid id)
        {
            try
            {
                // Lấy thông tin chủ đề trước khi xóa
                var chuDe = await _chuDeRepository.GetByIdAsync(id);
                if (chuDe == null)
                {
                    return ApiPhanHoi<ChuDe>.ThatBai("Không tìm thấy chủ đề với ID: " + id);
                }

                await _chuDeRepository.DeleteAsync(id);

                return ApiPhanHoi<ChuDe>.ThanhCongOk(chuDe, "Xóa chủ đề thành công");
            }
            catch (Exception ex)
            {
                return ApiPhanHoi<ChuDe>.ThatBai("Lỗi khi xóa chủ đề: " + ex.Message);
            }
        }

        public async Task<ApiPhanHoi<IEnumerable<ChuDe>>> GetHierarchicalTreeAsync()
        {
            try
            {
                var allChuDes = await _chuDeRepository.GetAllAsync();
                var rootChuDes = allChuDes.Where(c => c.ParentId == null).ToList();
                return ApiPhanHoi<IEnumerable<ChuDe>>.ThanhCongOk(rootChuDes, "Lấy cây phân cấp chủ đề thành công");
            }
            catch (Exception ex)
            {
                return ApiPhanHoi<IEnumerable<ChuDe>>.ThatBai("Lỗi khi lấy cây phân cấp chủ đề: " + ex.Message);
            }
        }    
    }
}
