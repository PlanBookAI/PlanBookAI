namespace PlanbookAI.FileStorageService.Data
{
    using Microsoft.EntityFrameworkCore;
    using PlanbookAI.FileStorageService.Models.Entities;
    
    public class FileStorageDbContext : DbContext
    {
        public FileStorageDbContext(DbContextOptions<FileStorageDbContext> options) : base(options) { }
        
        public DbSet<TepTin> TepTins { get; set; }
        public DbSet<TepTinMetadata> TepTinMetadatas { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // TepTin configuration
            modelBuilder.Entity<TepTin>(entity =>
            {
                entity.ToTable("file_storage", "files");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.FileName).HasColumnName("file_name").HasMaxLength(255);
                entity.Property(e => e.OriginalName).HasColumnName("original_name").HasMaxLength(255);
                entity.Property(e => e.FilePath).HasColumnName("file_path").HasMaxLength(500);
                entity.Property(e => e.FileSize).HasColumnName("file_size");
                entity.Property(e => e.MimeType).HasColumnName("mime_type").HasMaxLength(100);
                entity.Property(e => e.FileHash).HasColumnName("file_hash").HasMaxLength(64);
                entity.Property(e => e.UploadedBy).HasColumnName("uploaded_by");
                entity.Property(e => e.FileType).HasColumnName("file_type").HasMaxLength(50);
                entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(50);
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
                
                entity.HasIndex(e => e.FileHash);
                entity.HasIndex(e => e.UploadedBy);
                entity.HasIndex(e => e.Status);
            });
            
            // TepTinMetadata configuration
            modelBuilder.Entity<TepTinMetadata>(entity =>
            {
                entity.ToTable("file_metadata", "files");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.FileId).HasColumnName("file_id");
                entity.Property(e => e.Key).HasColumnName("key").HasMaxLength(100);
                entity.Property(e => e.Value).HasColumnName("value");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
                
                entity.HasOne(e => e.File)
                      .WithMany(e => e.Metadata)
                      .HasForeignKey(e => e.FileId);
                      
                entity.HasIndex(e => new { e.FileId, e.Key }).IsUnique();
            });
        }
    }
}