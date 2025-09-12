namespace PlanbookAI.FileStorageService.Repositories
{
    using Microsoft.EntityFrameworkCore;
    using PlanbookAI.FileStorageService.Data;
    using PlanbookAI.FileStorageService.Models.Entities;
    
    public class TepTinRepository : ITepTinRepository
    {
        private readonly FileStorageDbContext _context;
        
        public TepTinRepository(FileStorageDbContext context)
        {
            _context = context;
        }
        
        public async Task<TepTin?> GetByIdAsync(Guid id)
        {
            return await _context.TepTins
                .Include(t => t.Metadata)
                .FirstOrDefaultAsync(t => t.Id == id);
        }
        
        public async Task<TepTin?> GetByHashAndUserAsync(string hash, Guid userId, string originalName)
        {
            return await _context.TepTins
                .Include(t => t.Metadata)
                .FirstOrDefaultAsync(t => t.FileHash == hash && 
                                        t.UploadedBy == userId && 
                                        t.OriginalName == originalName &&
                                        t.Status != "DELETED");
        }
        
        public async Task<(List<TepTin> files, int total)> GetPagedAsync(int page, int size, string? fileType, string? status, Guid? userId)
        {
            var query = _context.TepTins.Include(t => t.Metadata).AsQueryable();
            
            if (userId.HasValue)
                query = query.Where(t => t.UploadedBy == userId.Value);
                
            if (!string.IsNullOrEmpty(fileType))
                query = query.Where(t => t.FileType == fileType);
                
            if (!string.IsNullOrEmpty(status))
                query = query.Where(t => t.Status == status);
            else
                query = query.Where(t => t.Status != "DELETED");
            
            var total = await query.CountAsync();
            var files = await query
                .OrderByDescending(t => t.CreatedAt)
                .Skip((page - 1) * size)
                .Take(size)
                .ToListAsync();
                
            return (files, total);
        }
        
        public async Task<List<TepTin>> SearchByMetadataAsync(string key, string value, Guid? userId)
        {
            var query = _context.TepTins
                .Include(t => t.Metadata)
                .Where(t => t.Metadata.Any(m => m.Key == key && m.Value.Contains(value)) &&
                           t.Status != "DELETED");
                           
            if (userId.HasValue)
                query = query.Where(t => t.UploadedBy == userId.Value);
                
            return await query.OrderByDescending(t => t.CreatedAt).ToListAsync();
        }
        
        public async Task<TepTin> CreateAsync(TepTin tepTin)
        {
            _context.TepTins.Add(tepTin);
            await _context.SaveChangesAsync();
            return tepTin;
        }
        
        public async Task UpdateAsync(TepTin tepTin)
        {
            tepTin.UpdatedAt = DateTime.UtcNow;
            _context.TepTins.Update(tepTin);
            await _context.SaveChangesAsync();
        }
        
        public async Task DeleteAsync(Guid id)
        {
            var tepTin = await _context.TepTins.FindAsync(id);
            if (tepTin != null)
            {
                _context.TepTins.Remove(tepTin);
                await _context.SaveChangesAsync();
            }
        }
        
        public async Task<List<TepTinMetadata>> GetMetadataAsync(Guid fileId)
        {
            return await _context.TepTinMetadatas
                .Where(m => m.FileId == fileId)
                .ToListAsync();
        }
        
        public async Task UpsertMetadataAsync(Guid fileId, string key, string value)
        {
            var existing = await _context.TepTinMetadatas
                .FirstOrDefaultAsync(m => m.FileId == fileId && m.Key == key);
                
            if (existing != null)
            {
                existing.Value = value;
            }
            else
            {
                _context.TepTinMetadatas.Add(new TepTinMetadata
                {
                    Id = Guid.NewGuid(),
                    FileId = fileId,
                    Key = key,
                    Value = value
                });
            }
            
            await _context.SaveChangesAsync();
        }
        
        public async Task DeleteMetadataAsync(Guid fileId, string key)
        {
            var metadata = await _context.TepTinMetadatas
                .FirstOrDefaultAsync(m => m.FileId == fileId && m.Key == key);
                
            if (metadata != null)
            {
                _context.TepTinMetadatas.Remove(metadata);
                await _context.SaveChangesAsync();
            }
        }
    }
}