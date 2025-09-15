using Microsoft.EntityFrameworkCore;
using ClassroomService.Data;
using ClassroomService.Models.Entities;
using ClassroomService.Repositories.Interfaces;

namespace ClassroomService.Repositories.Implementations
{
    /// <summary>
    /// Repository class for managing AnswerSheets entity database operations
    /// </summary>
    public class AnswerSheetsRepository : IAnswerSheetsRepository
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of AnswerSheetsRepository
        /// </summary>
        public AnswerSheetsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets an answer sheet by ID
        /// </summary>
        public async Task<AnswerSheets?> GetByIdAsync(int id)
        {
            return await _context.AnswerSheets
                .Include(a => a.Student)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        /// <summary>
        /// Gets answer sheets by student ID with pagination
        /// </summary>
        public async Task<IEnumerable<AnswerSheets>> GetByStudentIdAsync(int studentId, int page = 1, int pageSize = 10)
        {
            return await _context.AnswerSheets
                .Include(a => a.Student)
                .Where(a => a.StudentId == studentId)
                .OrderByDescending(a => a.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        /// <summary>
        /// Gets answer sheets by exam ID with pagination
        /// </summary>
        public async Task<IEnumerable<AnswerSheets>> GetByExamIdAsync(int examId, int page = 1, int pageSize = 10)
        {
            return await _context.AnswerSheets
                .Include(a => a.Student)
                .Where(a => a.ExamId == examId)
                .OrderByDescending(a => a.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        /// <summary>
        /// Creates a new answer sheet in the database
        /// </summary>
        public async Task<AnswerSheets> CreateAsync(AnswerSheets entity)
        {
            entity.CreatedAt = DateTime.UtcNow;
            
            _context.AnswerSheets.Add(entity);
            await _context.SaveChangesAsync();
            
            // Load the student information
            await _context.Entry(entity)
                .Reference(a => a.Student)
                .LoadAsync();
                
            return entity;
        }

        /// <summary>
        /// Updates an existing answer sheet in the database
        /// </summary>
        public async Task<AnswerSheets> UpdateAsync(AnswerSheets entity)
        {
            _context.AnswerSheets.Update(entity);
            await _context.SaveChangesAsync();
            
            // Load the student information
            await _context.Entry(entity)
                .Reference(a => a.Student)
                .LoadAsync();
                
            return entity;
        }

        /// <summary>
        /// Gets total count of answer sheets
        /// </summary>
        public async Task<int> GetTotalCountAsync(int? studentId = null, int? examId = null)
        {
            var query = _context.AnswerSheets.AsQueryable();
            
            if (studentId.HasValue)
            {
                query = query.Where(a => a.StudentId == studentId);
            }
            
            if (examId.HasValue)
            {
                query = query.Where(a => a.ExamId == examId);
            }
            
            return await query.CountAsync();
        }
    }
}
