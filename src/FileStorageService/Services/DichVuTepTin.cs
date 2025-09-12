namespace PlanbookAI.FileStorageService.Services
{
    using System.Security.Cryptography;
    using System.Text.Json;
    using PlanbookAI.FileStorageService.Models.DTOs;
    using PlanbookAI.FileStorageService.Models.Entities;
    using PlanbookAI.FileStorageService.Repositories;
    
    public class DichVuTepTin : IDichVuTepTin
    {
        private readonly ITepTinRepository _repository;
        private readonly ILogger<DichVuTepTin> _logger;
        private readonly string _storageRoot;
        private readonly int _maxUploadMB;
        private readonly List<string> _allowedMimes;
        
        private readonly Dictionary<string, List<string>> _fileTypeMimes = new()
        {
            ["IMAGE"] = new() { "image/jpeg", "image/png", "image/gif", "image/webp", "image/bmp" },
            ["DOCUMENT"] = new() { "text/plain", "application/msword", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
            ["PDF"] = new() { "application/pdf" },
            ["EXCEL"] = new() { "application/vnd.ms-excel", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
            ["OTHER"] = new()
        };
        
        public DichVuTepTin(ITepTinRepository repository, ILogger<DichVuTepTin> logger)
        {
            _repository = repository;
            _logger = logger;
            _storageRoot = Environment.GetEnvironmentVariable("FILE_STORAGE_ROOT") ?? "./data/files";
            _maxUploadMB = int.Parse(Environment.GetEnvironmentVariable("MAX_UPLOAD_MB") ?? "50");
            _allowedMimes = (Environment.GetEnvironmentVariable("ALLOWED_MIME") ?? 
                "image/jpeg,image/png,application/pdf,text/plain").Split(',').ToList();
        }
        
        public async Task<PhanHoiTepTin> UploadAsync(YeuCauUploadTepTin request, Guid userId)
        {
            // Validate file size
            if (!IsValidFileSize(request.Tep.Length))
            {
                throw new ArgumentException($"Kích thước tệp vượt quá giới hạn {_maxUploadMB}MB");
            }
            
            // Detect file type
            var fileType = DetectFileType(request.Tep.ContentType, request.Loai);
            
            // Validate MIME type
            if (!IsValidMimeType(request.Tep.ContentType, fileType))
            {
                throw new ArgumentException("Định dạng tệp không được hỗ trợ");
            }
            
            // Validate file name for security
            if (IsUnsafeFileName(request.Tep.FileName))
            {
                throw new ArgumentException("Tên tệp không an toàn");
            }
            
            // Calculate hash
            string fileHash;
            using (var stream = request.Tep.OpenReadStream())
            {
                fileHash = ComputeFileHash(stream);
            }
            
            // Check for duplicates
            var existingFile = await _repository.GetByHashAndUserAsync(fileHash, userId, request.Tep.FileName);
            if (existingFile != null)
            {
                return MapToResponse(existingFile, true);
            }
            
            // Generate file name and path
            var fileExtension = Path.GetExtension(request.Tep.FileName);
            var fileName = $"{Guid.NewGuid()}{fileExtension}";
            var dateFolder = DateTime.UtcNow.ToString("yyyy/MM/dd");
            var relativePath = Path.Combine(dateFolder, fileName);
            var fullPath = Path.Combine(_storageRoot, relativePath);
            
            // Ensure directory exists
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
            
            // Save file
            using (var fileStream = new FileStream(fullPath, FileMode.Create))
            {
                await request.Tep.CopyToAsync(fileStream);
            }
            
            // Create entity
            var tepTin = new TepTin
            {
                Id = Guid.NewGuid(),
                FileName = fileName,
                OriginalName = request.Tep.FileName,
                FilePath = relativePath,
                FileSize = request.Tep.Length,
                MimeType = request.Tep.ContentType,
                FileHash = fileHash,
                UploadedBy = userId,
                FileType = fileType,
                Status = "ACTIVE"
            };
            
            // Save to database
            var savedFile = await _repository.CreateAsync(tepTin);
            
            // Process metadata if provided
            if (!string.IsNullOrEmpty(request.Metadata))
            {
                try
                {
                    var metadataDict = JsonSerializer.Deserialize<Dictionary<string, string>>(request.Metadata);
                    if (metadataDict != null)
                    {
                        foreach (var kvp in metadataDict)
                        {
                            await _repository.UpsertMetadataAsync(savedFile.Id, kvp.Key, kvp.Value);
                        }
                    }
                }
                catch (JsonException ex)
                {
                    _logger.LogWarning("Invalid metadata JSON: {Error}", ex.Message);
                }
            }
            
            _logger.LogInformation("File uploaded: {FileId} by {UserId}", savedFile.Id, userId);
            
            return MapToResponse(savedFile);
        }
        
        public async Task<TepTin?> GetByIdAsync(Guid id, Guid? userId, bool isAdmin)
        {
            var file = await _repository.GetByIdAsync(id);
            
            if (file == null || file.Status == "DELETED")
                return null;
                
            if (!isAdmin && userId.HasValue && file.UploadedBy != userId.Value)
                return null;
                
            return file;
        }
        
        public async Task<PhanHoiDanhSachTepTin> GetPagedAsync(int page, int size, string? fileType, string? status, Guid? userId, bool isAdmin)
        {
            var searchUserId = isAdmin ? null : userId;
            var (files, total) = await _repository.GetPagedAsync(page, size, fileType, status, searchUserId);
            
            return new PhanHoiDanhSachTepTin
            {
                DanhSach = files.Select(f => MapToResponse(f)).ToList(),
                TongSo = total,
                Trang = page,
                KichThuoc = size,
                TongTrang = (int)Math.Ceiling((double)total / size)
            };
        }
        
        public async Task<List<PhanHoiTepTin>> SearchByMetadataAsync(string key, string value, Guid? userId, bool isAdmin)
        {
            var searchUserId = isAdmin ? null : userId;
            var files = await _repository.SearchByMetadataAsync(key, value, searchUserId);
            return files.Select(f => MapToResponse(f)).ToList();
        }
        
        public async Task<bool> SoftDeleteAsync(Guid id, Guid userId, bool isAdmin)
        {
            var file = await _repository.GetByIdAsync(id);
            if (file == null || file.Status == "DELETED")
                return false;
                
            if (!isAdmin && file.UploadedBy != userId)
                return false;
                
            file.Status = "DELETED";
            await _repository.UpdateAsync(file);
            
            _logger.LogInformation("File soft deleted: {FileId} by {UserId}", id, userId);
            return true;
        }
        
        public async Task<bool> RestoreAsync(Guid id, Guid userId, bool isAdmin)
        {
            var file = await _repository.GetByIdAsync(id);
            if (file == null || file.Status != "DELETED")
                return false;
                
            if (!isAdmin && file.UploadedBy != userId)
                return false;
                
            file.Status = "ACTIVE";
            await _repository.UpdateAsync(file);
            
            _logger.LogInformation("File restored: {FileId} by {UserId}", id, userId);
            return true;
        }
        
        public async Task<bool> PermanentDeleteAsync(Guid id, bool isAdmin)
        {
            if (!isAdmin)
                return false;
                
            var file = await _repository.GetByIdAsync(id);
            if (file == null)
                return false;
                
            // Delete physical file
            var fullPath = Path.Combine(_storageRoot, file.FilePath);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
            
            // Delete from database
            await _repository.DeleteAsync(id);
            
            _logger.LogInformation("File permanently deleted: {FileId}", id);
            return true;
        }
        
        public async Task<bool> UpdateMetadataAsync(Guid id, Dictionary<string, string> metadata, Guid userId, bool isAdmin)
        {
            var file = await _repository.GetByIdAsync(id);
            if (file == null || file.Status == "DELETED")
                return false;
                
            if (!isAdmin && file.UploadedBy != userId)
                return false;
                
            foreach (var kvp in metadata)
            {
                await _repository.UpsertMetadataAsync(id, kvp.Key, kvp.Value);
            }
            
            _logger.LogInformation("File metadata updated: {FileId} by {UserId}", id, userId);
            return true;
        }
        
        public bool IsValidMimeType(string mimeType, string? fileType)
        {
            if (!_allowedMimes.Contains(mimeType))
                return false;
                
            if (!string.IsNullOrEmpty(fileType) && _fileTypeMimes.ContainsKey(fileType))
            {
                var typeMimes = _fileTypeMimes[fileType];
                if (typeMimes.Any() && !typeMimes.Contains(mimeType))
                    return false;
            }
            
            return true;
        }
        
        public bool IsValidFileSize(long size)
        {
            return size <= _maxUploadMB * 1024 * 1024;
        }
        
        public string ComputeFileHash(Stream stream)
        {
            using var sha256 = SHA256.Create();
            var hash = sha256.ComputeHash(stream);
            return Convert.ToHexString(hash).ToLowerInvariant();
        }
        
        private string DetectFileType(string mimeType, string? requestedType)
        {
            if (!string.IsNullOrEmpty(requestedType) && _fileTypeMimes.ContainsKey(requestedType))
                return requestedType;
                
            foreach (var kvp in _fileTypeMimes)
            {
                if (kvp.Value.Contains(mimeType))
                    return kvp.Key;
            }
            
            return "OTHER";
        }
        
        private bool IsUnsafeFileName(string fileName)
        {
            var unsafeExtensions = new[] { ".exe", ".bat", ".cmd", ".scr", ".com", ".pif" };
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            
            return unsafeExtensions.Contains(extension) || 
                   fileName.Contains("..") || 
                   fileName.Contains("/") || 
                   fileName.Contains("\\");
        }
        
        private PhanHoiTepTin MapToResponse(TepTin file, bool isDuplicate = false)
        {
            return new PhanHoiTepTin
            {
                Id = file.Id,
                TenGoc = file.OriginalName,
                KichThuoc = file.FileSize,
                MimeType = file.MimeType,
                Loai = file.FileType,
                TrangThai = file.Status,
                Hash = file.FileHash,
                NgayTao = file.CreatedAt,
                ThuocTinh = file.Metadata?.ToDictionary(m => m.Key, m => m.Value),
                IsDuplicate = isDuplicate ? true : null
            };
        }
    }
}