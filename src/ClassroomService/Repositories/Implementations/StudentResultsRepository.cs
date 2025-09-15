using Microsoft.EntityFrameworkCore;
using ClassroomService.Data;
using ClassroomService.Models.Entities;
using ClassroomService.Repositories.Interfaces;

namespace ClassroomService.Repositories.Implementations
{
    /// <summary>
    /// Repository class for managing StudentResults entity database operations
    /// </summary>
    public class StudentResultsRepository : IStudentResultsRepository
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of StudentResultsRepository
        /// </summary>
        public StudentResultsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets a student result by ID
        /// </summary>
        public async Task<StudentResults?> GetByIdAsync(Guid id)
        {
            return await _context.StudentResults
                .Include(sr => sr.Student)
                .FirstOrDefaultAsync(sr => sr.Id == id);
        }

        /// <summary>
        /// Gets student results by student ID with pagination
        /// </summary>
        public async Task<IEnumerable<StudentResults>> GetByStudentIdAsync(Guid studentId, int page = 1, int pageSize = 10)
        {
            return await _context.StudentResults
                .Include(sr => sr.Student)
                .Where(sr => sr.StudentId == studentId)
                .OrderByDescending(sr => sr.ExamDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        /// <summary>
        /// Gets student results by exam ID with pagination
        /// </summary>
        public async Task<IEnumerable<StudentResults>> GetByExamIdAsync(Guid examId, int page = 1, int pageSize = 10)
        {
            return await _context.StudentResults
                .Include(sr => sr.Student)
                .Where(sr => sr.ExamId == examId)
                .OrderByDescending(sr => sr.ExamDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        /// <summary>
        /// Creates a new student result in the database
        /// </summary>
        public async Task<StudentResults> CreateAsync(StudentResults entity)
        {
            entity.GradedAt = DateTime.UtcNow;
            
            _context.StudentResults.Add(entity);
            await _context.SaveChangesAsync();
            
            // Load the student information
            await _context.Entry(entity)
                .Reference(sr => sr.Student)
                .LoadAsync();
                
            return entity;
        }

        /// <summary>
        /// Updates an existing student result in the database
        /// </summary>
        public async Task<StudentResults> UpdateAsync(StudentResults entity)
        {
            entity.GradedAt = DateTime.UtcNow;
            
            _context.StudentResults.Update(entity);
            await _context.SaveChangesAsync();
            
            // Load the student information
            await _context.Entry(entity)
                .Reference(sr => sr.Student)
                .LoadAsync();
                
            return entity;
        }

        /// <summary>
        /// Gets total count of student results
        /// </summary>
        public async Task<int> GetTotalCountAsync(Guid? studentId = null, Guid? examId = null)
        {
            var query = _context.StudentResults.AsQueryable();
            
            if (studentId.HasValue)
            {
                query = query.Where(sr => sr.StudentId == studentId);
            }
            
            if (examId.HasValue)
            {
                query = query.Where(sr => sr.ExamId == examId);
            }
            
            return await query.CountAsync();
        }
    }
}
