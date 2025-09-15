using Microsoft.EntityFrameworkCore;
using ClassroomService.Data;
using ClassroomService.Models.Entities;
using ClassroomService.Repositories.Interfaces;

namespace ClassroomService.Repositories.Implementations
{
    /// <summary>
    /// Repository class for managing Classes entity database operations
    /// </summary>
    public class ClassesRepository : IClassesRepository
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of ClassesRepository
        /// </summary>
        public ClassesRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets a class by ID, optionally filtered by homeroom teacher ID
        /// </summary>
        public async Task<Classes?> GetByIdAsync(Guid id, Guid? homeroomTeacherId = null)
        {
            var query = _context.Classes.AsQueryable();
            
            if (homeroomTeacherId.HasValue)
            {
                query = query.Where(c => c.HomeroomTeacherId == homeroomTeacherId);
            }
            
            return await query.FirstOrDefaultAsync(c => c.Id == id);
        }

        /// <summary>
        /// Gets all classes with pagination and optional homeroom teacher filter
        /// </summary>
        public async Task<IEnumerable<Classes>> GetAllAsync(Guid? homeroomTeacherId = null, int page = 1, int pageSize = 10)
        {
            var query = _context.Classes.AsQueryable();
            
            if (homeroomTeacherId.HasValue)
            {
                query = query.Where(c => c.HomeroomTeacherId == homeroomTeacherId);
            }
            
            return await query
                .OrderBy(c => c.Grade)
                .ThenBy(c => c.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        /// <summary>
        /// Gets classes by homeroom teacher ID with pagination
        /// </summary>
        public async Task<IEnumerable<Classes>> GetByHomeroomTeacherIdAsync(Guid homeroomTeacherId, int page = 1, int pageSize = 10)
        {
            return await _context.Classes
                .Where(c => c.HomeroomTeacherId == homeroomTeacherId)
                .OrderBy(c => c.Grade)
                .ThenBy(c => c.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        /// <summary>
        /// Creates a new class in the database
        /// </summary>
        public async Task<Classes> CreateAsync(Classes entity)
        {
            entity.CreatedAt = DateTime.UtcNow;
            entity.UpdatedAt = DateTime.UtcNow;
            
            _context.Classes.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        /// <summary>
        /// Updates an existing class in the database
        /// </summary>
        public async Task<Classes> UpdateAsync(Classes entity)
        {
            entity.UpdatedAt = DateTime.UtcNow;
            
            _context.Classes.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        /// <summary>
        /// Deletes a class by ID, ensuring only homeroom teacher can delete
        /// </summary>
        public async Task<bool> DeleteAsync(Guid id, Guid homeroomTeacherId)
        {
            var entity = await _context.Classes
                .FirstOrDefaultAsync(c => c.Id == id && c.HomeroomTeacherId == homeroomTeacherId);
                
            if (entity == null) return false;
            
            _context.Classes.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Gets total count of classes, optionally filtered by homeroom teacher ID
        /// </summary>
        public async Task<int> GetTotalCountAsync(Guid? homeroomTeacherId = null)
        {
            var query = _context.Classes.AsQueryable();
            
            if (homeroomTeacherId.HasValue)
            {
                query = query.Where(c => c.HomeroomTeacherId == homeroomTeacherId);
            }
            
            return await query.CountAsync();
        }
    }
}