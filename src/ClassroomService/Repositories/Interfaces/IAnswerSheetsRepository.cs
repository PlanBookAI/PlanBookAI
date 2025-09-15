using ClassroomService.Models.Entities;

namespace ClassroomService.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for AnswerSheets entity operations
    /// </summary>
    public interface IAnswerSheetsRepository
    {
        /// <summary>
        /// Get answer sheet by ID
        /// </summary>
        Task<AnswerSheets?> GetByIdAsync(int id);
        
        /// <summary>
        /// Get answer sheets by student ID with pagination
        /// </summary>
        Task<IEnumerable<AnswerSheets>> GetByStudentIdAsync(int studentId, int page = 1, int pageSize = 10);
        
        /// <summary>
        /// Get answer sheets by exam ID with pagination
        /// </summary>
        Task<IEnumerable<AnswerSheets>> GetByExamIdAsync(int examId, int page = 1, int pageSize = 10);
        
        /// <summary>
        /// Create new answer sheet
        /// </summary>
        Task<AnswerSheets> CreateAsync(AnswerSheets entity);
        
        /// <summary>
        /// Update existing answer sheet
        /// </summary>
        Task<AnswerSheets> UpdateAsync(AnswerSheets entity);
        
        /// <summary>
        /// Get total count of answer sheets
        /// </summary>
        Task<int> GetTotalCountAsync(int? studentId = null, int? examId = null);
    }
}
