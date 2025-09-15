using Microsoft.EntityFrameworkCore;
using ClassroomService.Data;
using ClassroomService.Models.Entities;
using ClassroomService.Repositories.Interfaces;

namespace ClassroomService.Repositories.Implementations
{
    /// <summary>
    /// Repository class for managing Students entity database operations
    /// </summary>
    public class StudentsRepository : IStudentsRepository
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of StudentsRepository
        /// </summary>
        public StudentsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets a student by ID with optional owner teacher filter
        /// </summary>
        public async Task<Students?> GetByIdAsync(Guid id, Guid? ownerTeacherId = null)
        {
            var query = _context.Students.Include(s => s.Class).AsQueryable();
            
            if (ownerTeacherId.HasValue)
            {
                query = query.Where(s => s.OwnerTeacherId == ownerTeacherId);
            }
            
            return await query.FirstOrDefaultAsync(s => s.Id == id);
        }

        /// <summary>
        /// Gets students by class ID with pagination and optional owner teacher filter
        /// </summary>
        public async Task<IEnumerable<Students>> GetByClassIdAsync(Guid classId, Guid? ownerTeacherId = null, int page = 1, int pageSize = 10)
        {
            var query = _context.Students.Include(s => s.Class).Where(s => s.ClassId == classId);
            
            if (ownerTeacherId.HasValue)
            {
                query = query.Where(s => s.OwnerTeacherId == ownerTeacherId);
            }
            
            return await query
                .OrderBy(s => s.FullName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        /// <summary>
        /// Gets students by owner teacher ID with pagination
        /// </summary>
        public async Task<IEnumerable<Students>> GetByOwnerTeacherIdAsync(Guid ownerTeacherId, int page = 1, int pageSize = 10)
        {
            return await _context.Students
                .Include(s => s.Class)
                .Where(s => s.OwnerTeacherId == ownerTeacherId)
                .OrderBy(s => s.FullName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        /// <summary>
        /// Creates a new student in the database
        /// </summary>
        public async Task<Students> CreateAsync(Students entity)
        {
            entity.CreatedAt = DateTime.UtcNow;
            entity.UpdatedAt = DateTime.UtcNow;
            
            _context.Students.Add(entity);
            await _context.SaveChangesAsync();
            
            // Load the class information
            await _context.Entry(entity)
                .Reference(s => s.Class)
                .LoadAsync();
                
            return entity;
        }

        /// <summary>
        /// Updates an existing student in the database
        /// </summary>
        public async Task<Students> UpdateAsync(Students entity)
        {
            entity.UpdatedAt = DateTime.UtcNow;
            
            _context.Students.Update(entity);
            await _context.SaveChangesAsync();
            
            // Load the class information
            await _context.Entry(entity)
                .Reference(s => s.Class)
                .LoadAsync();
                
            return entity;
        }

        /// <summary>
        /// Deletes a student by ID, ensuring only owner teacher can delete
        /// </summary>
        public async Task<bool> DeleteAsync(Guid id, Guid ownerTeacherId)
        {
            var entity = await _context.Students
                .FirstOrDefaultAsync(s => s.Id == id && s.OwnerTeacherId == ownerTeacherId);
                
            if (entity == null) return false;
            
            _context.Students.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Checks if a student code already exists, optionally excluding a specific student
        /// </summary>
        public async Task<bool> ExistsByStudentCodeAsync(string studentCode, Guid? excludeId = null)
        {
            var query = _context.Students.Where(s => s.StudentCode == studentCode);
            
            if (excludeId.HasValue)
            {
                query = query.Where(s => s.Id != excludeId);
            }
            
            return await query.AnyAsync();
        }

        /// <summary>
        /// Gets total count of students with optional filters
        /// </summary>
        public async Task<int> GetTotalCountAsync(Guid? classId = null, Guid? ownerTeacherId = null)
        {
            var query = _context.Students.AsQueryable();
            
            if (classId.HasValue)
            {
                query = query.Where(s => s.ClassId == classId);
            }
            
            if (ownerTeacherId.HasValue)
            {
                query = query.Where(s => s.OwnerTeacherId == ownerTeacherId);
            }
            
            return await query.CountAsync();
        }
    }
}