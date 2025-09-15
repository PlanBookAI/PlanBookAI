using ClassroomService.Models.DTOs;

namespace ClassroomService.Services.Interfaces
{
    /// <summary>
    /// Service interface for managing answer sheets
    /// </summary>
    public interface IAnswerSheetsService
    {
        /// <summary>
        /// Creates a new answer sheet
        /// </summary>
        Task<AnswerSheetDto> LuuBaiLam(CreateAnswerSheetDto dto);
        
        /// <summary>
        /// Gets answer sheets by student ID
        /// </summary>
        Task<(IEnumerable<AnswerSheetDto> Items, int TotalCount)> LayBaiLamTheoHocSinh(int studentId, int page = 1, int pageSize = 10);
        
        /// <summary>
        /// Gets answer sheets by exam ID
        /// </summary>
        Task<(IEnumerable<AnswerSheetDto> Items, int TotalCount)> LayBaiLamTheoDeThi(int examId, int page = 1, int pageSize = 10);
        
        /// <summary>
        /// Updates an existing answer sheet
        /// </summary>
        Task<AnswerSheetDto> CapNhatBaiLam(int id, CreateAnswerSheetDto dto);
        
        /// <summary>
        /// Gets an answer sheet by ID
        /// </summary>
        Task<AnswerSheetDto?> LayBaiLamTheoId(int id);
    }
}
