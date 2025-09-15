using ClassroomService.Models.DTOs;

namespace ClassroomService.Services.Interfaces
{
    /// <summary>
    /// Service interface for managing classes
    /// </summary>
    public interface IClassesService
    {
        /// <summary>
        /// Creates a new class
        /// </summary>
        Task<ClassDto> TaoLopHoc(CreateClassDto dto, Guid homeroomTeacherId);
        
        /// <summary>
        /// Updates an existing class
        /// </summary>
        Task<ClassDto> CapNhatLopHoc(Guid id, UpdateClassDto dto, Guid homeroomTeacherId);
        
        /// <summary>
        /// Deletes a class
        /// </summary>
        Task<bool> XoaLopHoc(Guid id, Guid homeroomTeacherId);
        
        /// <summary>
        /// Gets paginated list of classes
        /// </summary>
        Task<(IEnumerable<ClassDto> Items, int TotalCount)> LayDanhSachLopHoc(Guid? homeroomTeacherId = null, int page = 1, int pageSize = 10);
        
        /// <summary>
        /// Gets a class by ID
        /// </summary>
        Task<ClassDto> LayLopHocTheoId(Guid id, Guid? homeroomTeacherId = null);
        
        /// <summary>
        /// Gets classes by homeroom teacher
        /// </summary>
        Task<(IEnumerable<ClassDto> Items, int TotalCount)> LayLopHocTheoGiaoVien(Guid homeroomTeacherId, int page = 1, int pageSize = 10);
    }
}