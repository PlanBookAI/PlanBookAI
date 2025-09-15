using ClassroomService.Models.DTOs;

namespace ClassroomService.Services.Interfaces
{
    /// <summary>
    /// Service interface for managing students
    /// </summary>
    public interface IStudentsService
    {
        /// <summary>
        /// Gets paginated list of students
        /// </summary>
        Task<(IEnumerable<StudentDto> Items, int TotalCount)> LayDanhSachHocSinh(Guid? ownerTeacherId = null, int page = 1, int pageSize = 10);
        
        /// <summary>
        /// Gets a student by ID
        /// </summary>
        Task<StudentDto> LayHocSinhTheoId(Guid id, Guid? ownerTeacherId = null);
        
        /// <summary>
        /// Gets students by class ID with pagination
        /// </summary>
        Task<(IEnumerable<StudentDto> Items, int TotalCount)> LayHocSinhTheoLop(Guid classId, Guid? ownerTeacherId = null, int page = 1, int pageSize = 10);
        
        /// <summary>
        /// Creates a new student
        /// </summary>
        Task<StudentDto> ThemHocSinh(CreateStudentDto dto);
        
        /// <summary>
        /// Updates an existing student
        /// </summary>
        Task<StudentDto> CapNhatHocSinh(Guid id, UpdateStudentDto dto, Guid ownerTeacherId);
        
        /// <summary>
        /// Deletes a student
        /// </summary>
        Task<bool> XoaHocSinh(Guid id, Guid ownerTeacherId);
    }
}