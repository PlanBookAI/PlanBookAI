# Detailed Design Document (DDD) - PlanbookAI

## 1. Tổng quan

### 1.1 Mục đích
Tài liệu này mô tả chi tiết thiết kế hệ thống PlanbookAI sử dụng .NET Core, bao gồm kiến trúc, components, và implementation details.

### 1.2 Phạm vi
- Backend API sử dụng .NET Core
- Microservices architecture
- Entity Framework Core cho database
- JWT authentication
- Integration với AI và OCR services

## 2. Kiến trúc tổng thể

### 2.1 Solution Structure

```
PlanbookAI.sln
├── src/
│   ├── PlanbookAI.Gateway/           # API Gateway
│   ├── PlanbookAI.DichVuXacThuc/     # Authentication Service
│   ├── PlanbookAI.DichVuNguoiDung/   # User Management
│   ├── PlanbookAI.DichVuGiaoAn/      # Lesson Plan Service
│   ├── PlanbookAI.DichVuDeThi/       # Task & Assessment Service
│   └── PlanbookAI.Shared/            # Shared Libraries
├── tests/
│   ├── PlanbookAI.DichVuXacThuc.Tests/
│   ├── PlanbookAI.DichVuNguoiDung.Tests/
│   ├── PlanbookAI.DichVuGiaoAn.Tests/
│   └── PlanbookAI.DichVuDeThi.Tests/
└── docs/
    ├── api-documentation/
    └── deployment-guides/
```

### 2.2 Project Structure cho mỗi Service

```
PlanbookAI.DichVuXacThuc/
├── Controllers/
│   ├── XacThucController.cs
│   └── HealthController.cs
├── Services/
│   ├── IDichVuXacThuc.cs
│   ├── DichVuXacThuc.cs
│   ├── IDichVuJWT.cs
│   └── DichVuJWT.cs
├── Models/
│   ├── DTOs/
│   │   ├── YeuCauDangNhap.cs
│   │   ├── PhanHoiDangNhap.cs
│   │   └── YeuCauDangKy.cs
│   └── Entities/
│       ├── NguoiDung.cs
│       └── VaiTro.cs
├── Data/
│   ├── INguoiDungRepository.cs
│   ├── NguoiDungRepository.cs
│   └── NguoiDungDbContext.cs
├── Middleware/
│   ├── JwtMiddleware.cs
│   └── ExceptionMiddleware.cs
├── Program.cs
└── appsettings.json
```

## 3. Domain Models

### 3.1 User Domain

```csharp
// Entities
public class NguoiDung
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string MatKhauMaHoa { get; set; }
    public string HoTen { get; set; }
    public VaiTro VaiTro { get; set; }
    public TrangThaiNguoiDung TrangThai { get; set; }
    public DateTime ThoiGianTao { get; set; }
    public DateTime ThoiGianCapNhat { get; set; }
}

public enum VaiTro
{
    Admin,
    Manager,
    Staff,
    Teacher
}

public enum TrangThaiNguoiDung
{
    HoatDong,
    KhongHoatDong,
    TamKhoa
}

// DTOs
public class YeuCauDangNhap
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    
    [Required]
    [MinLength(8)]
    public string MatKhau { get; set; }
}

public class PhanHoiDangNhap
{
    public string Token { get; set; }
    public string RefreshToken { get; set; }
    public DateTime ThoiGianHetHan { get; set; }
    public ThongTinNguoiDungDto NguoiDung { get; set; }
}

public class ThongTinNguoiDungDto
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string HoTen { get; set; }
    public VaiTro VaiTro { get; set; }
}
```

### 3.2 Educational Content Domain

```csharp
// Entities
public class GiaoAn
{
    public Guid Id { get; set; }
    public string TieuDe { get; set; }
    public string NoiDung { get; set; }
    public Guid GiaoVienId { get; set; }
    public Guid? MauGiaoAnId { get; set; }
    public TrangThaiGiaoAn TrangThai { get; set; }
    public DateTime ThoiGianTao { get; set; }
    public DateTime ThoiGianCapNhat { get; set; }
    
    // Navigation properties
    public NguoiDung GiaoVien { get; set; }
    public MauGiaoAn MauGiaoAn { get; set; }
}

public class MauGiaoAn
{
    public Guid Id { get; set; }
    public string Ten { get; set; }
    public string NoiDung { get; set; }
    public string MonHoc { get; set; }
    public int KhoiLop { get; set; }
    public bool HoatDong { get; set; }
    public DateTime ThoiGianTao { get; set; }
}

public enum TrangThaiGiaoAn
{
    BanNhap,
    DaXuatBan,
    DaLuuTru
}

// DTOs
public class YeuCauTaoGiaoAn
{
    [Required]
    [StringLength(200)]
    public string TieuDe { get; set; }
    
    [Required]
    public string NoiDung { get; set; }
    
    public Guid? MauGiaoAnId { get; set; }
}

public class GiaoAnDto
{
    public Guid Id { get; set; }
    public string TieuDe { get; set; }
    public string NoiDung { get; set; }
    public Guid GiaoVienId { get; set; }
    public string TenGiaoVien { get; set; }
    public TrangThaiGiaoAn TrangThai { get; set; }
    public DateTime ThoiGianTao { get; set; }
}
```

### 3.3 Assessment Domain

```csharp
// Entities
public class CauHoi
{
    public Guid Id { get; set; }
    public string NoiDung { get; set; }
    public LoaiCauHoi Loai { get; set; }
    public MucDoKho MucDoKho { get; set; }
    public string MonHoc { get; set; }
    public string ChuDe { get; set; }
    public List<LuaChonCauHoi> DanhSachLuaChon { get; set; }
    public string DapAnDung { get; set; }
    public Guid NguoiTaoId { get; set; }
    public DateTime ThoiGianTao { get; set; }
    public DateTime ThoiGianCapNhat { get; set; }
}

public class LuaChonCauHoi
{
    public Guid Id { get; set; }
    public Guid CauHoiId { get; set; }
    public string NoiDung { get; set; }
    public string Nhan { get; set; } // A, B, C, D
}

public class DeThi
{
    public Guid Id { get; set; }
    public string TieuDe { get; set; }
    public Guid GiaoVienId { get; set; }
    public int ThoiGianLam { get; set; } // phút
    public int TongSoCauHoi { get; set; }
    public TrangThaiDeThi TrangThai { get; set; }
    public DateTime ThoiGianTao { get; set; }
    
    // Navigation properties
    public NguoiDung GiaoVien { get; set; }
    public List<CauHoiTrongDeThi> DanhSachCauHoi { get; set; }
}

public class CauHoiTrongDeThi
{
    public Guid DeThiId { get; set; }
    public Guid CauHoiId { get; set; }
    public int Diem { get; set; }
    public int ThuTu { get; set; }
    
    // Navigation properties
    public DeThi DeThi { get; set; }
    public CauHoi CauHoi { get; set; }
}

public enum LoaiCauHoi
{
    TracNghiem,
    DungSai,
    TuLuan,
    DienKhoangTrong
}

public enum MucDoKho
{
    De,
    TrungBinh,
    Kho,
    RatKho
}

public enum TrangThaiDeThi
{
    BanNhap,
    DaXuatBan,
    HoanThanh,
    DaLuuTru
}

// DTOs
public class YeuCauTaoCauHoi
{
    [Required]
    public string NoiDung { get; set; }
    
    [Required]
    public LoaiCauHoi Loai { get; set; }
    
    [Required]
    public MucDoKho MucDoKho { get; set; }
    
    [Required]
    public string MonHoc { get; set; }
    
    [Required]
    public string ChuDe { get; set; }
    
    public List<LuaChonCauHoiDto> DanhSachLuaChon { get; set; }
    
    [Required]
    public string DapAnDung { get; set; }
}

public class YeuCauTaoDeThi
{
    [Required]
    [StringLength(200)]
    public string TieuDe { get; set; }
    
    [Required]
    [Range(15, 180)]
    public int ThoiGianLam { get; set; }
    
    [Required]
    [Range(1, 100)]
    public int TongSoCauHoi { get; set; }
    
    public List<Guid> DanhSachCauHoiId { get; set; }
}
```

### 3.4 Student Data Domain

```csharp
// Entities
public class HocSinh
{
    public Guid Id { get; set; }
    public string HoTen { get; set; }
    public string MaSoHocSinh { get; set; }
    public Guid GiaoVienId { get; set; }
    public Guid? LopHocId { get; set; }
    public DateTime ThoiGianTao { get; set; }
    
    // Navigation properties
    public NguoiDung GiaoVien { get; set; }
    public LopHoc LopHoc { get; set; }
    public List<KetQuaHocSinh> DanhSachKetQua { get; set; }
}

public class KetQuaHocSinh
{
    public Guid Id { get; set; }
    public Guid HocSinhId { get; set; }
    public Guid DeThiId { get; set; }
    public decimal Diem { get; set; }
    public string DapAn { get; set; } // JSON
    public DateTime ThoiGianCham { get; set; }
    
    // Navigation properties
    public HocSinh HocSinh { get; set; }
    public DeThi DeThi { get; set; }
}

public class LopHoc
{
    public Guid Id { get; set; }
    public string Ten { get; set; }
    public int KhoiLop { get; set; }
    public Guid GiaoVienId { get; set; }
    public DateTime ThoiGianTao { get; set; }
    
    // Navigation properties
    public NguoiDung GiaoVien { get; set; }
    public List<HocSinh> DanhSachHocSinh { get; set; }
}

// DTOs
public class YeuCauTaoHocSinh
{
    [Required]
    [StringLength(100)]
    public string HoTen { get; set; }
    
    [Required]
    [StringLength(20)]
    public string MaSoHocSinh { get; set; }
    
    public Guid? LopHocId { get; set; }
}

public class HocSinhDto
{
    public Guid Id { get; set; }
    public string HoTen { get; set; }
    public string MaSoHocSinh { get; set; }
    public string TenLopHoc { get; set; }
    public DateTime ThoiGianTao { get; set; }
}
```

## 4. Database Design

### 4.1 Entity Framework Configuration

```csharp
// NguoiDungDbContext.cs
public class NguoiDungDbContext : DbContext
{
    public NguoiDungDbContext(DbContextOptions<NguoiDungDbContext> options) : base(options)
    {
    }
    
    public DbSet<NguoiDung> DanhSachNguoiDung { get; set; }
    public DbSet<VaiTro> DanhSachVaiTro { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // NguoiDung configuration
        modelBuilder.Entity<NguoiDung>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.MatKhauMaHoa).IsRequired();
            entity.Property(e => e.VaiTro).IsRequired();
            entity.Property(e => e.TrangThai).IsRequired();
            entity.Property(e => e.ThoiGianTao).IsRequired();
            entity.Property(e => e.ThoiGianCapNhat).IsRequired();
        });
    }
}

// GiaoDucDbContext.cs
public class GiaoDucDbContext : DbContext
{
    public GiaoDucDbContext(DbContextOptions<GiaoDucDbContext> options) : base(options)
    {
    }
    
    public DbSet<GiaoAn> DanhSachGiaoAn { get; set; }
    public DbSet<MauGiaoAn> DanhSachMauGiaoAn { get; set; }
    public DbSet<CauHoi> DanhSachCauHoi { get; set; }
    public DbSet<LuaChonCauHoi> DanhSachLuaChonCauHoi { get; set; }
    public DbSet<DeThi> DanhSachDeThi { get; set; }
    public DbSet<CauHoiTrongDeThi> DanhSachCauHoiTrongDeThi { get; set; }
    public DbSet<HocSinh> DanhSachHocSinh { get; set; }
    public DbSet<KetQuaHocSinh> DanhSachKetQuaHocSinh { get; set; }
    public DbSet<LopHoc> DanhSachLopHoc { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // CauHoi configuration
        modelBuilder.Entity<CauHoi>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.NoiDung).IsRequired();
            entity.Property(e => e.Loai).IsRequired();
            entity.Property(e => e.MucDoKho).IsRequired();
            entity.Property(e => e.MonHoc).IsRequired().HasMaxLength(50);
            entity.Property(e => e.ChuDe).IsRequired().HasMaxLength(100);
            entity.Property(e => e.DapAnDung).IsRequired();
            entity.Property(e => e.ThoiGianTao).IsRequired();
            entity.Property(e => e.ThoiGianCapNhat).IsRequired();
            
            entity.HasOne(e => e.NguoiTao)
                  .WithMany()
                  .HasForeignKey(e => e.NguoiTaoId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
        
        // LuaChonCauHoi configuration
        modelBuilder.Entity<LuaChonCauHoi>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.NoiDung).IsRequired();
            entity.Property(e => e.Nhan).IsRequired().HasMaxLength(10);
            
            entity.HasOne(e => e.CauHoi)
                  .WithMany(q => q.DanhSachLuaChon)
                  .HasForeignKey(e => e.CauHoiId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
        
        // DeThi configuration
        modelBuilder.Entity<DeThi>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TieuDe).IsRequired().HasMaxLength(200);
            entity.Property(e => e.ThoiGianLam).IsRequired();
            entity.Property(e => e.TongSoCauHoi).IsRequired();
            entity.Property(e => e.TrangThai).IsRequired();
            entity.Property(e => e.ThoiGianTao).IsRequired();
            
            entity.HasOne(e => e.GiaoVien)
                  .WithMany()
                  .HasForeignKey(e => e.GiaoVienId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
        
        // CauHoiTrongDeThi configuration
        modelBuilder.Entity<CauHoiTrongDeThi>(entity =>
        {
            entity.HasKey(e => new { e.DeThiId, e.CauHoiId });
            entity.Property(e => e.Diem).IsRequired();
            entity.Property(e => e.ThuTu).IsRequired();
            
            entity.HasOne(e => e.DeThi)
                  .WithMany(ex => ex.DanhSachCauHoi)
                  .HasForeignKey(e => e.DeThiId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasOne(e => e.CauHoi)
                  .WithMany()
                  .HasForeignKey(e => e.CauHoiId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
```

### 4.2 Database Migrations

```csharp
// Initial migration
public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Create Users table
        migrationBuilder.CreateTable(
            name: "Users",
            columns: table => new
            {
                Id = table.Column<Guid>(nullable: false),
                Email = table.Column<string>(maxLength: 100, nullable: false),
                PasswordHash = table.Column<string>(nullable: false),
                FirstName = table.Column<string>(maxLength: 50, nullable: false),
                LastName = table.Column<string>(maxLength: 50, nullable: false),
                Role = table.Column<int>(nullable: false),
                Status = table.Column<int>(nullable: false),
                CreatedAt = table.Column<DateTime>(nullable: false),
                UpdatedAt = table.Column<DateTime>(nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Users", x => x.Id);
            });
            
        migrationBuilder.CreateIndex(
            name: "IX_Users_Email",
            table: "Users",
            column: "Email",
            unique: true);
    }
    
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "Users");
    }
}
```

## 5. Service Layer Design

### 5.1 Authentication Service

```csharp
// IDichVuXacThuc.cs
public interface IDichVuXacThuc
{
    Task<PhanHoiDangNhap> DangNhapAsync(YeuCauDangNhap yeuCau);
    Task<PhanHoiDangNhap> LamMoiTokenAsync(string refreshToken);
    Task<bool> KiemTraTokenAsync(string token);
    Task<ThongTinNguoiDungDto> LayNguoiDungTheoIdAsync(Guid nguoiDungId);
    Task<bool> DoiMatKhauAsync(Guid nguoiDungId, string matKhauHienTai, string matKhauMoi);
}

// DichVuXacThuc.cs
public class DichVuXacThuc : IDichVuXacThuc
{
    private readonly INguoiDungRepository _nguoiDungRepository;
    private readonly IDichVuJWT _dichVuJWT;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<DichVuXacThuc> _logger;
    
    public DichVuXacThuc(
        INguoiDungRepository nguoiDungRepository,
        IDichVuJWT dichVuJWT,
        IPasswordHasher passwordHasher,
        ILogger<DichVuXacThuc> logger)
    {
        _nguoiDungRepository = nguoiDungRepository;
        _dichVuJWT = dichVuJWT;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }
    
    public async Task<PhanHoiDangNhap> DangNhapAsync(YeuCauDangNhap yeuCau)
    {
        var nguoiDung = await _nguoiDungRepository.LayTheoEmailAsync(yeuCau.Email);
        if (nguoiDung == null)
        {
            throw new UnauthorizedException("Email hoặc mật khẩu không đúng");
        }
        
        if (!_passwordHasher.KiemTraMatKhau(yeuCau.MatKhau, nguoiDung.MatKhauMaHoa))
        {
            throw new UnauthorizedException("Email hoặc mật khẩu không đúng");
        }
        
        if (nguoiDung.TrangThai != TrangThaiNguoiDung.HoatDong)
        {
            throw new UnauthorizedException("Tài khoản đã bị khóa");
        }
        
        var token = _dichVuJWT.TaoToken(nguoiDung);
        var refreshToken = _dichVuJWT.TaoRefreshToken();
        
        // Cập nhật lần đăng nhập cuối
        nguoiDung.LanDangNhapCuoi = DateTime.UtcNow;
        await _nguoiDungRepository.CapNhatAsync(nguoiDung);
        
        return new PhanHoiDangNhap
        {
            Token = token,
            RefreshToken = refreshToken,
            ThoiGianHetHan = DateTime.UtcNow.AddHours(24),
            NguoiDung = new ThongTinNguoiDungDto
            {
                Id = nguoiDung.Id,
                Email = nguoiDung.Email,
                HoTen = nguoiDung.HoTen,
                VaiTro = nguoiDung.VaiTro
            }
        };
    }
    
    public async Task<PhanHoiDangNhap> LamMoiTokenAsync(string refreshToken)
    {
        var nguoiDungId = _dichVuJWT.KiemTraRefreshToken(refreshToken);
        var nguoiDung = await _nguoiDungRepository.LayTheoIdAsync(nguoiDungId);
        
        if (nguoiDung == null || nguoiDung.TrangThai != TrangThaiNguoiDung.HoatDong)
        {
            throw new UnauthorizedException("Token không hợp lệ");
        }
        
        var tokenMoi = _dichVuJWT.TaoToken(nguoiDung);
        var refreshTokenMoi = _dichVuJWT.TaoRefreshToken();
        
        return new PhanHoiDangNhap
        {
            Token = tokenMoi,
            RefreshToken = refreshTokenMoi,
            ThoiGianHetHan = DateTime.UtcNow.AddHours(24),
            NguoiDung = new ThongTinNguoiDungDto
            {
                Id = nguoiDung.Id,
                Email = nguoiDung.Email,
                HoTen = nguoiDung.HoTen,
                VaiTro = nguoiDung.VaiTro
            }
        };
    }
}
```

### 5.2 Question Service

```csharp
// IDichVuCauHoi.cs
public interface IDichVuCauHoi
{
    Task<CauHoiDto> TaoAsync(YeuCauTaoCauHoi yeuCau, Guid giaoVienId);
    Task<CauHoiDto> LayTheoIdAsync(Guid id);
    Task<KetQuaPhanTrang<CauHoiDto>> LayTheoGiaoVienAsync(Guid giaoVienId, BoLocCauHoi boLoc);
    Task<CauHoiDto> CapNhatAsync(Guid id, YeuCauCapNhatCauHoi yeuCau, Guid giaoVienId);
    Task<bool> XoaAsync(Guid id, Guid giaoVienId);
    Task<List<CauHoiDto>> LayTheoDeThiAsync(Guid deThiId);
}

// DichVuCauHoi.cs
public class DichVuCauHoi : IDichVuCauHoi
{
    private readonly ICauHoiRepository _cauHoiRepository;
    private readonly ILogger<DichVuCauHoi> _logger;
    
    public DichVuCauHoi(ICauHoiRepository cauHoiRepository, ILogger<DichVuCauHoi> logger)
    {
        _cauHoiRepository = cauHoiRepository;
        _logger = logger;
    }
    
    public async Task<CauHoiDto> TaoAsync(YeuCauTaoCauHoi yeuCau, Guid giaoVienId)
    {
        var cauHoi = new CauHoi
        {
            Id = Guid.NewGuid(),
            NoiDung = yeuCau.NoiDung,
            Loai = yeuCau.Loai,
            MucDoKho = yeuCau.MucDoKho,
            MonHoc = yeuCau.MonHoc,
            ChuDe = yeuCau.ChuDe,
            DapAnDung = yeuCau.DapAnDung,
            NguoiTaoId = giaoVienId,
            ThoiGianTao = DateTime.UtcNow,
            ThoiGianCapNhat = DateTime.UtcNow
        };
        
        if (yeuCau.DanhSachLuaChon != null && yeuCau.DanhSachLuaChon.Any())
        {
            cauHoi.DanhSachLuaChon = yeuCau.DanhSachLuaChon.Select((luaChon, index) => new LuaChonCauHoi
            {
                Id = Guid.NewGuid(),
                CauHoiId = cauHoi.Id,
                NoiDung = luaChon.NoiDung,
                Nhan = luaChon.Nhan
            }).ToList();
        }
        
        await _cauHoiRepository.TaoAsync(cauHoi);
        
        return new CauHoiDto
        {
            Id = cauHoi.Id,
            NoiDung = cauHoi.NoiDung,
            Loai = cauHoi.Loai,
            MucDoKho = cauHoi.MucDoKho,
            MonHoc = cauHoi.MonHoc,
            ChuDe = cauHoi.ChuDe,
            DanhSachLuaChon = cauHoi.DanhSachLuaChon?.Select(l => new LuaChonCauHoiDto
            {
                Id = l.Id,
                NoiDung = l.NoiDung,
                Nhan = l.Nhan
            }).ToList(),
            DapAnDung = cauHoi.DapAnDung,
            ThoiGianTao = cauHoi.ThoiGianTao
        };
    }
    
    public async Task<KetQuaPhanTrang<CauHoiDto>> LayTheoGiaoVienAsync(Guid giaoVienId, BoLocCauHoi boLoc)
    {
        var cauHois = await _cauHoiRepository.LayTheoGiaoVienAsync(giaoVienId, boLoc);
        
        return new KetQuaPhanTrang<CauHoiDto>
        {
            DanhSach = cauHois.DanhSach.Select(q => new CauHoiDto
            {
                Id = q.Id,
                NoiDung = q.NoiDung,
                Loai = q.Loai,
                MucDoKho = q.MucDoKho,
                MonHoc = q.MonHoc,
                ChuDe = q.ChuDe,
                DanhSachLuaChon = q.DanhSachLuaChon?.Select(l => new LuaChonCauHoiDto
                {
                    Id = l.Id,
                    NoiDung = l.NoiDung,
                    Nhan = l.Nhan
                }).ToList(),
                DapAnDung = q.DapAnDung,
                ThoiGianTao = q.ThoiGianTao
            }).ToList(),
            TongSo = cauHois.TongSo,
            TrangHienTai = cauHois.TrangHienTai,
            KichThuocTrang = cauHois.KichThuocTrang
        };
    }
}
```

## 6. Controller Design

### 6.1 Authentication Controller

```csharp
// XacThucController.cs
[ApiController]
[Route("api/v1/xac-thuc")]
public class XacThucController : ControllerBase
{
    private readonly IDichVuXacThuc _dichVuXacThuc;
    private readonly ILogger<XacThucController> _logger;
    
    public XacThucController(IDichVuXacThuc dichVuXacThuc, ILogger<XacThucController> logger)
    {
        _dichVuXacThuc = dichVuXacThuc;
        _logger = logger;
    }
    
    [HttpPost("dang-nhap")]
    public async Task<ActionResult<PhanHoiApi<PhanHoiDangNhap>>> DangNhap([FromBody] YeuCauDangNhap yeuCau)
    {
        try
        {
            var phanHoi = await _dichVuXacThuc.DangNhapAsync(yeuCau);
            return Ok(new PhanHoiApi<PhanHoiDangNhap>
            {
                ThanhCong = true,
                DuLieu = phanHoi,
                ThongBao = "Đăng nhập thành công"
            });
        }
        catch (UnauthorizedException ex)
        {
            return Unauthorized(new PhanHoiApi<object>
            {
                ThanhCong = false,
                ThongBao = ex.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi đăng nhập");
            return StatusCode(500, new PhanHoiApi<object>
            {
                ThanhCong = false,
                ThongBao = "Lỗi hệ thống"
            });
        }
    }
    
    [HttpPost("lam-moi-token")]
    public async Task<ActionResult<PhanHoiApi<PhanHoiDangNhap>>> LamMoiToken([FromBody] YeuCauLamMoiToken yeuCau)
    {
        try
        {
            var phanHoi = await _dichVuXacThuc.LamMoiTokenAsync(yeuCau.RefreshToken);
            return Ok(new PhanHoiApi<PhanHoiDangNhap>
            {
                ThanhCong = true,
                DuLieu = phanHoi,
                ThongBao = "Làm mới token thành công"
            });
        }
        catch (UnauthorizedException ex)
        {
            return Unauthorized(new PhanHoiApi<object>
            {
                ThanhCong = false,
                ThongBao = ex.Message
            });
        }
    }
    
    [Authorize]
    [HttpGet("thong-tin-ca-nhan")]
    public async Task<ActionResult<PhanHoiApi<ThongTinNguoiDungDto>>> LayThongTinCaNhan()
    {
        var nguoiDungId = User.GetNguoiDungId();
        var nguoiDung = await _dichVuXacThuc.LayNguoiDungTheoIdAsync(nguoiDungId);
        
        return Ok(new PhanHoiApi<ThongTinNguoiDungDto>
        {
            ThanhCong = true,
            DuLieu = nguoiDung,
            ThongBao = "Lấy thông tin người dùng thành công"
        });
    }
}
```

### 6.2 Question Controller

```csharp
// CauHoiController.cs
[ApiController]
[Route("api/v1/cau-hoi")]
[Authorize]
public class CauHoiController : ControllerBase
{
    private readonly IDichVuCauHoi _dichVuCauHoi;
    private readonly ILogger<CauHoiController> _logger;
    
    public CauHoiController(IDichVuCauHoi dichVuCauHoi, ILogger<CauHoiController> logger)
    {
        _dichVuCauHoi = dichVuCauHoi;
        _logger = logger;
    }
    
    [HttpPost]
    [Authorize(Roles = "Staff,Teacher")]
    public async Task<ActionResult<PhanHoiApi<CauHoiDto>>> Tao([FromBody] YeuCauTaoCauHoi yeuCau)
    {
        try
        {
            var giaoVienId = User.GetNguoiDungId();
            var cauHoi = await _dichVuCauHoi.TaoAsync(yeuCau, giaoVienId);
            
            return CreatedAtAction(nameof(LayTheoId), new { id = cauHoi.Id }, new PhanHoiApi<CauHoiDto>
            {
                ThanhCong = true,
                DuLieu = cauHoi,
                ThongBao = "Tạo câu hỏi thành công"
            });
        }
        catch (ValidationException ex)
        {
            return BadRequest(new PhanHoiApi<object>
            {
                ThanhCong = false,
                ThongBao = ex.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi tạo câu hỏi");
            return StatusCode(500, new PhanHoiApi<object>
            {
                ThanhCong = false,
                ThongBao = "Lỗi hệ thống"
            });
        }
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<PhanHoiApi<CauHoiDto>>> LayTheoId(Guid id)
    {
        try
        {
            var cauHoi = await _dichVuCauHoi.LayTheoIdAsync(id);
            if (cauHoi == null)
            {
                return NotFound(new PhanHoiApi<object>
                {
                    ThanhCong = false,
                    ThongBao = "Không tìm thấy câu hỏi"
                });
            }
            
            return Ok(new PhanHoiApi<CauHoiDto>
            {
                ThanhCong = true,
                DuLieu = cauHoi,
                ThongBao = "Lấy câu hỏi thành công"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi lấy câu hỏi");
            return StatusCode(500, new PhanHoiApi<object>
            {
                ThanhCong = false,
                ThongBao = "Lỗi hệ thống"
            });
        }
    }
    
    [HttpGet]
    public async Task<ActionResult<PhanHoiApi<KetQuaPhanTrang<CauHoiDto>>>> LayTheoGiaoVien(
        [FromQuery] BoLocCauHoi boLoc)
    {
        try
        {
            var giaoVienId = User.GetNguoiDungId();
            var cauHois = await _dichVuCauHoi.LayTheoGiaoVienAsync(giaoVienId, boLoc);
            
            return Ok(new PhanHoiApi<KetQuaPhanTrang<CauHoiDto>>
            {
                ThanhCong = true,
                DuLieu = cauHois,
                ThongBao = "Lấy danh sách câu hỏi thành công"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi lấy danh sách câu hỏi");
            return StatusCode(500, new PhanHoiApi<object>
            {
                ThanhCong = false,
                ThongBao = "Lỗi hệ thống"
            });
        }
    }
}
```

## 7. Middleware Design

### 7.1 JWT Middleware

```csharp
// JwtMiddleware.cs
public class JwtMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IDichVuJWT _dichVuJWT;
    
    public JwtMiddleware(RequestDelegate next, IDichVuJWT dichVuJWT)
    {
        _next = next;
        _dichVuJWT = dichVuJWT;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        
        if (token != null)
        {
            try
            {
                var nguoiDungId = _dichVuJWT.KiemTraToken(token);
                context.Items["NguoiDungId"] = nguoiDungId;
            }
            catch
            {
                // Token không hợp lệ, nhưng không throw exception
                // để cho phép anonymous endpoints
            }
        }
        
        await _next(context);
    }
}

// DichVuJWT.cs
public class DichVuJWT : IDichVuJWT
{
    private readonly IConfiguration _configuration;
    
    public DichVuJWT(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public string TaoToken(NguoiDung nguoiDung)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"]);
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, nguoiDung.Id.ToString()),
                new Claim(ClaimTypes.Email, nguoiDung.Email),
                new Claim(ClaimTypes.Role, nguoiDung.VaiTro.ToString())
            }),
            Expires = DateTime.UtcNow.AddHours(24),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };
        
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
    
    public Guid KiemTraToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"]);
        
        tokenHandler.ValidateToken(token, new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero
        }, out SecurityToken validatedToken);
        
        var jwtToken = (JwtSecurityToken)validatedToken;
        var nguoiDungId = Guid.Parse(jwtToken.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);
        
        return nguoiDungId;
    }
}
```

### 7.2 Exception Middleware

```csharp
// ExceptionMiddleware.cs
public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    
    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            await XuLyExceptionAsync(context, ex);
        }
    }
    
    private static async Task XuLyExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        
        var phanHoi = new PhanHoiApi<object>
        {
            ThanhCong = false,
            ThongBao = "Lỗi hệ thống"
        };
        
        switch (exception)
        {
            case ValidationException:
                context.Response.StatusCode = 400;
                phanHoi.ThongBao = exception.Message;
                break;
            case UnauthorizedException:
                context.Response.StatusCode = 401;
                phanHoi.ThongBao = exception.Message;
                break;
            case NotFoundException:
                context.Response.StatusCode = 404;
                phanHoi.ThongBao = exception.Message;
                break;
            default:
                context.Response.StatusCode = 500;
                break;
        }
        
        var result = JsonSerializer.Serialize(phanHoi);
        await context.Response.WriteAsync(result);
    }
}
```

## 8. Configuration

### 8.1 appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=planbookai;Username=postgres;Password=password"
  },
  "Jwt": {
    "Secret": "your-super-secret-key-with-at-least-32-characters",
    "Issuer": "PlanbookAI",
    "Audience": "PlanbookAI",
    "ExpiryInHours": 24
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:3000",
      "https://planbookai.vercel.app"
    ]
  },
  "ExternalServices": {
    "GeminiAI": {
      "ApiKey": "your-gemini-api-key",
      "BaseUrl": "https://generativelanguage.googleapis.com"
    },
    "GoogleVision": {
      "ApiKey": "your-google-vision-api-key",
      "BaseUrl": "https://vision.googleapis.com"
    }
  }
}
```

### 8.2 Program.cs

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add DbContext
builder.Services.AddDbContext<NguoiDungDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDbContext<GiaoDucDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Repositories
builder.Services.AddScoped<INguoiDungRepository, NguoiDungRepository>();
builder.Services.AddScoped<ICauHoiRepository, CauHoiRepository>();
builder.Services.AddScoped<IDeThiRepository, DeThiRepository>();

// Add Services
builder.Services.AddScoped<IDichVuXacThuc, DichVuXacThuc>();
builder.Services.AddScoped<IDichVuCauHoi, DichVuCauHoi>();
builder.Services.AddScoped<IDichVuDeThi, DichVuDeThi>();
builder.Services.AddScoped<IDichVuJWT, DichVuJWT>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins",
        policy =>
        {
            policy.WithOrigins(builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>())
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowSpecificOrigins");
app.UseMiddleware<JwtMiddleware>();
app.UseMiddleware<ExceptionMiddleware>();
app.UseAuthorization();
app.MapControllers();

app.Run();
```

## 9. Testing Strategy

### 9.1 Unit Tests

```csharp
// DichVuXacThucTests.cs
public class DichVuXacThucTests
{
    private readonly Mock<INguoiDungRepository> _mockNguoiDungRepository;
    private readonly Mock<IDichVuJWT> _mockDichVuJWT;
    private readonly Mock<IPasswordHasher> _mockPasswordHasher;
    private readonly Mock<ILogger<DichVuXacThuc>> _mockLogger;
    private readonly DichVuXacThuc _dichVuXacThuc;
    
    public DichVuXacThucTests()
    {
        _mockNguoiDungRepository = new Mock<INguoiDungRepository>();
        _mockDichVuJWT = new Mock<IDichVuJWT>();
        _mockPasswordHasher = new Mock<IPasswordHasher>();
        _mockLogger = new Mock<ILogger<DichVuXacThuc>>();
        
        _dichVuXacThuc = new DichVuXacThuc(
            _mockNguoiDungRepository.Object,
            _mockDichVuJWT.Object,
            _mockPasswordHasher.Object,
            _mockLogger.Object);
    }
    
    [Fact]
    public async Task DangNhapAsync_VoiThongTinHopLe_ReturnsLoginResponse()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "teacher@example.com",
            Password = "password123"
        };
        
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "teacher@example.com",
            PasswordHash = "hashedPassword",
            Role = UserRole.Teacher,
            Status = UserStatus.Active
        };
        
        _mockNguoiDungRepository.Setup(x => x.GetByEmailAsync(request.Email))
            .ReturnsAsync(user);
        
        _mockPasswordHasher.Setup(x => x.VerifyPassword(request.Password, user.PasswordHash))
            .Returns(true);
        
        _mockDichVuJWT.Setup(x => x.GenerateToken(user))
            .Returns("jwt-token");
        
        _mockDichVuJWT.Setup(x => x.GenerateRefreshToken())
            .Returns("refresh-token");
        
        // Act
        var result = await _dichVuXacThuc.LoginAsync(request);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal("jwt-token", result.Token);
        Assert.Equal("refresh-token", result.RefreshToken);
        Assert.Equal(user.Email, result.User.Email);
        Assert.Equal(user.Role, result.User.Role);
    }
    
    [Fact]
    public async Task DangNhapAsync_VoiThongTinKhongHopLe_ThrowsUnauthorizedException()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "teacher@example.com",
            Password = "wrongpassword"
        };
        
        _mockNguoiDungRepository.Setup(x => x.GetByEmailAsync(request.Email))
            .ReturnsAsync((User)null);
        
        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedException>(() => 
            _dichVuXacThuc.LoginAsync(request));
    }
}
```

### 9.2 Integration Tests

```csharp
// XacThucControllerIntegrationTests.cs
public class XacThucControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    
    public XacThucControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }
    
    [Fact]
    public async Task DangNhap_VoiThongTinHopLe_ReturnsOkResult()
    {
        // Arrange
        var client = _factory.CreateClient();
        var yeuCau = new LoginRequest
        {
            Email = "teacher@example.com",
            Password = "password123"
        };
        
        var json = JsonSerializer.Serialize(yeuCau);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        // Act
        var response = await client.PostAsync("/api/v1/xac-thuc/dang-nhap", content);
        
        // Assert
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<PhanHoiApi<PhanHoiDangNhap>>(responseContent);
        
        Assert.True(result.ThanhCong);
        Assert.NotNull(result.DuLieu.Token);
        Assert.NotNull(result.DuLieu.RefreshToken);
    }
}
```

## 10. Deployment Configuration

### 10.1 Dockerfile

```dockerfile
# Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["PlanbookAI.DichVuXacThuc/PlanbookAI.DichVuXacThuc.csproj", "PlanbookAI.DichVuXacThuc/"]
COPY ["PlanbookAI.Shared/PlanbookAI.Shared.csproj", "PlanbookAI.Shared/"]
RUN dotnet restore "PlanbookAI.DichVuXacThuc/PlanbookAI.DichVuXacThuc.csproj"
COPY . .
WORKDIR "/src/PlanbookAI.DichVuXacThuc"
RUN dotnet build "PlanbookAI.DichVuXacThuc.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PlanbookAI.DichVuXacThuc.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PlanbookAI.DichVuXacThuc.dll"]
```

### 10.2 docker-compose.yml

```yaml
version: '3.8'

services:
  dich-vu-xac-thuc:
    build:
      context: .
      dockerfile: PlanbookAI.DichVuXacThuc/Dockerfile
    ports:
      - "8081:80"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ConnectionStrings__DefaultConnection=Host=postgres;Database=planbookai;Username=postgres;Password=password
    depends_on:
      - postgres

  dich-vu-nguoi-dung:
    build:
      context: .
      dockerfile: PlanbookAI.DichVuNguoiDung/Dockerfile
    ports:
      - "8082:80"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ConnectionStrings__DefaultConnection=Host=postgres;Database=planbookai;Username=postgres;Password=password
    depends_on:
      - postgres

  dich-vu-giao-an:
    build:
      context: .
      dockerfile: PlanbookAI.DichVuGiaoAn/Dockerfile
    ports:
      - "8083:80"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ConnectionStrings__DefaultConnection=Host=postgres;Database=planbookai;Username=postgres;Password=password
    depends_on:
      - postgres

  dich-vu-de-thi:
    build:
      context: .
      dockerfile: PlanbookAI.DichVuDeThi/Dockerfile
    ports:
      - "8084:80"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ConnectionStrings__DefaultConnection=Host=postgres;Database=planbookai;Username=postgres;Password=password
    depends_on:
      - postgres

  gateway-service:
    build:
      context: .
      dockerfile: PlanbookAI.Gateway/Dockerfile
    ports:
      - "8080:80"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
    depends_on:
      - dich-vu-xac-thuc
      - dich-vu-nguoi-dung
      - dich-vu-giao-an
      - dich-vu-de-thi

  postgres:
    image: postgres:15
    environment:
      POSTGRES_DB: planbookai
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: password
    ports:
      - "5432:5432"
    volumes:
      - postgres_data:/var/lib/postgresql/data

volumes:
  postgres_data:
```

## 11. Security Implementation

### 11.1 Role-Based Access Control

```csharp
// CauHinhBaoMat.cs
[Configuration]
[EnableWebSecurity]
[EnableGlobalMethodSecurity(prePostEnabled = true)]
public class CauHinhBaoMat : WebSecurityConfigurerAdapter
{
    protected override void Configure(HttpSecurity http) throws Exception
    {
        http.cors().and().csrf().disable()
            .exceptionHandling()
                .authenticationEntryPoint(unauthorizedHandler)
                .and()
            .sessionManagement()
                .sessionCreationPolicy(SessionCreationPolicy.STATELESS)
                .and()
            .authorizeRequests()
                .antMatchers("/api/v1/xac-thuc/**").permitAll()
                .antMatchers(HttpMethod.GET, "/api/v1/cau-hoi/**").hasAnyRole("TEACHER", "STAFF", "MANAGER", "ADMIN")
                .antMatchers(HttpMethod.POST, "/api/v1/cau-hoi/**").hasAnyRole("STAFF", "MANAGER", "ADMIN")
                .antMatchers("/api/v1/nguoi-dung/quan-ly/**").hasRole("ADMIN")
                .antMatchers("/api/v1/goi-dich-vu/**").hasAnyRole("MANAGER", "ADMIN")
                .anyRequest().authenticated();
        
        http.addFilterBefore(jwtAuthenticationFilter(), UsernamePasswordAuthenticationFilter.class);
    }
}

// Method-level security
[Service]
public class DichVuGiaoAn
{
    [PreAuthorize("hasRole('TEACHER') or hasRole('STAFF')")]
    public async Task<GiaoAn> TaoGiaoAn(YeuCauTaoGiaoAn yeuCau, Guid nguoiDungId)
    {
        // Logic tạo giáo án
    }
    
    [PreAuthorize("hasRole('ADMIN') or @dichVuGiaoAn.LaChuSoHuu(#id, authentication.principal.id)")]
    public async Task XoaGiaoAn(Guid id)
    {
        // Logic xóa giáo án
    }
    
    public bool LaChuSoHuu(Guid giaoAnId, Guid nguoiDungId)
    {
        return giaoAnRepository.TonTaiTheoIdVaGiaoVienId(giaoAnId, nguoiDungId);
    }
}
```

### 11.2 Data Encryption

```csharp
// DichVuMaHoa.cs
public class DichVuMaHoa : IDichVuMaHoa
{
    private static readonly string ALGORITHM = "AES/GCB/PKCS5PADDING";
    private static readonly string KEY = Configuration["App:Encryption:Key"];
    
    public string MaHoa(string duLieu)
    {
        if (duLieu == null) return null;
        
        try
        {
            Cipher cipher = Cipher.getInstance(ALGORITHM);
            SecretKeySpec keySpec = new SecretKeySpec(KEY.getBytes(), "AES");
            cipher.init(Cipher.ENCRYPT_MODE, keySpec);
            
            byte[] encrypted = cipher.doFinal(duLieu.getBytes());
            return Base64.getEncoder().encodeToString(encrypted);
        }
        catch (Exception e)
        {
            throw new RuntimeException("Lỗi mã hóa dữ liệu", e);
        }
    }
    
    public string GiaiMa(string duLieuMaHoa)
    {
        if (duLieuMaHoa == null) return null;
        
        try
        {
            Cipher cipher = Cipher.getInstance(ALGORITHM);
            SecretKeySpec keySpec = new SecretKeySpec(KEY.getBytes(), "AES");
            cipher.init(Cipher.DECRYPT_MODE, keySpec);
            
            byte[] decoded = Base64.getDecoder().decode(duLieuMaHoa);
            byte[] decrypted = cipher.doFinal(decoded);
            return new String(decrypted);
        }
        catch (Exception e)
        {
            throw new RuntimeException("Lỗi giải mã dữ liệu", e);
        }
    }
}

// Entity với encryption
public class HocSinh
{
    // ... other fields
    
    [Convert(typeof(StringEncryptionConverter))]
    public string HoTen { get; set; }
    
    [Convert(typeof(StringEncryptionConverter))]
    public string MaSoHocSinh { get; set; }
}
```

## 12. AI Integration

### 12.1 Gemini AI Service

```csharp
// IDichVuAI.cs
public interface IDichVuAI
{
    Task<string> TaoNoiDungGiaoAn(YeuCauTaoGiaoAn yeuCau);
    Task<List<CauHoi>> TaoNganHangCauHoi(YeuCauTaoCauHoi yeuCau);
    Task<string> CaiThienNoiDung(string noiDungGoc);
}

// DichVuGeminiAI.cs
public class DichVuGeminiAI : IDichVuAI
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<DichVuGeminiAI> _logger;
    
    public DichVuGeminiAI(HttpClient httpClient, IConfiguration configuration, ILogger<DichVuGeminiAI> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
    }
    
    [RetryPolicy(maxAttempts: 3)]
    public async Task<string> TaoNoiDungGiaoAn(YeuCauTaoGiaoAn yeuCau)
    {
        var prompt = XayDungPromptGiaoAn(yeuCau);
        
        try
        {
            var phanHoi = await GuiYeuCauDenGemini(prompt);
            return XuLyPhanHoiAI(phanHoi);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Lỗi khi gọi Gemini AI");
            throw new AIServiceException("Không thể tạo nội dung giáo án", e);
        }
    }
    
    private string XayDungPromptGiaoAn(YeuCauTaoGiaoAn yeuCau)
    {
        return $"""
            Tạo giáo án môn {yeuCau.MonHoc} cho lớp {yeuCau.Lop} với chủ đề: {yeuCau.ChuDe}
            
            Yêu cầu:
            - Thời gian: {yeuCau.ThoiGianDay} phút
            - Mục tiêu: {yeuCau.MucTieu}
            - Định dạng: Theo chương trình giáo dục Việt Nam
            - Ngôn ngữ: Tiếng Việt
            
            Cấu trúc mong muốn:
            1. Mục tiêu bài học
            2. Chuẩn bị
            3. Hoạt động dạy học
            4. Củng cố và đánh giá
            """;
    }
    
    private async Task<string> GuiYeuCauDenGemini(string prompt)
    {
        var request = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new { text = prompt }
                    }
                }
            }
        };
        
        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var apiKey = _configuration["ExternalServices:GeminiAI:ApiKey"];
        var baseUrl = _configuration["ExternalServices:GeminiAI:BaseUrl"];
        
        _httpClient.DefaultRequestHeaders.Add("x-goog-api-key", apiKey);
        
        var response = await _httpClient.PostAsync($"{baseUrl}/v1beta/models/gemini-pro:generateContent", content);
        
        if (!response.IsSuccessStatusCode)
        {
            throw new AIServiceException($"Lỗi API: {response.StatusCode}");
        }
        
        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<GeminiResponse>(responseContent);
        
        return result?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text ?? "";
    }
}
```

### 12.2 OCR Service

```csharp
// IDichVuOCR.cs
public interface IDichVuOCR
{
    Task<KetQuaOCR> XuLyBaiLam(IFormFile anhBaiLam, Guid deThiId);
    Task<ThongTinHocSinh> TrichXuatThongTinHocSinh(KetQuaOCR ketQua);
    Task<List<string>> TrichXuatDapAn(KetQuaOCR ketQua, Guid deThiId);
}

// DichVuGoogleVisionOCR.cs
public class DichVuGoogleVisionOCR : IDichVuOCR
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<DichVuGoogleVisionOCR> _logger;
    
    public DichVuGoogleVisionOCR(HttpClient httpClient, IConfiguration configuration, ILogger<DichVuGoogleVisionOCR> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
    }
    
    public async Task<KetQuaOCR> XuLyBaiLam(IFormFile anhBaiLam, Guid deThiId)
    {
        try
        {
            // 1. Upload ảnh lên S3
            var duongDanS3 = await LuuAnhLenS3(anhBaiLam);
            
            // 2. Xử lý OCR
            var ketQuaOCR = await ThucHienOCR(anhBaiLam);
            
            // 3. Trích xuất thông tin học sinh
            var thongTinHS = TrichXuatThongTinHocSinh(ketQuaOCR);
            
            // 4. Trích xuất đáp án
            var dapAn = await TrichXuatDapAn(ketQuaOCR, deThiId);
            
            // 5. Tính điểm
            var ketQua = await TinhDiem(dapAn, deThiId);
            
            // 6. Lưu kết quả
            await LuuKetQua(thongTinHS, ketQua, duongDanS3, deThiId);
            
            return ketQuaOCR;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Lỗi xử lý OCR");
            throw new OCRProcessingException("Không thể xử lý bài làm", e);
        }
    }
    
    private async Task<KetQuaOCR> ThucHienOCR(IFormFile anhData)
    {
        var imageBytes = await GetImageBytes(anhData);
        var base64Image = Convert.ToBase64String(imageBytes);
        
        var request = new
        {
            requests = new[]
            {
                new
                {
                    image = new
                    {
                        content = base64Image
                    },
                    features = new[]
                    {
                        new
                        {
                            type = "TEXT_DETECTION",
                            maxResults = 1
                        }
                    }
                }
            }
        };
        
        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var apiKey = _configuration["ExternalServices:GoogleVision:ApiKey"];
        var baseUrl = _configuration["ExternalServices:GoogleVision:BaseUrl"];
        
        var response = await _httpClient.PostAsync($"{baseUrl}/v1/images:annotate?key={apiKey}", content);
        
        if (!response.IsSuccessStatusCode)
        {
            throw new OCRProcessingException($"Lỗi Google Vision API: {response.StatusCode}");
        }
        
        var responseContent = await response.Content.ReadAsStringAsync();
        return XuLyPhanHoiOCR(responseContent);
    }
    
    public ThongTinHocSinh TrichXuatThongTinHocSinh(KetQuaOCR ketQuaOCR)
    {
        var text = ketQuaOCR.VanBanNhanDang;
        
        var tenPattern = new Regex(@"Họ tên:?\s*([\p{L}\s]+)");
        var mssvPattern = new Regex(@"MSSV:?\s*(\d+)");
        
        var tenMatch = tenPattern.Match(text);
        var mssvMatch = mssvPattern.Match(text);
        
        var hoTen = tenMatch.Success ? tenMatch.Groups[1].Value.Trim() : null;
        var mssv = mssvMatch.Success ? mssvMatch.Groups[1].Value.Trim() : null;
        
        return new ThongTinHocSinh(hoTen, mssv);
    }
    
    public async Task<List<string>> TrichXuatDapAn(KetQuaOCR ketQuaOCR, Guid deThiId)
    {
        var dapAn = new List<string>();
        
        // Logic phức tạp để nhận dạng các lựa chọn A, B, C, D
        // Sử dụng tọa độ và pattern matching
        for (int i = 1; i <= 50; i++)
        {
            var luaChon = NhanDangLuaChonCauHoi(ketQuaOCR, i);
            dapAn.Add(luaChon);
        }
        
        return dapAn;
    }
}
```

## 13. Monitoring và Logging

### 13.1 Custom Metrics

```csharp
// CustomMetrics.cs
public class CustomMetrics
{
    private readonly Counter _soLanDangNhap;
    private readonly Counter _soLanXuLyOCR;
    private readonly Timer _thoiGianPhanHoiAI;
    
    public CustomMetrics(MeterRegistry meterRegistry)
    {
        this._soLanDangNhap = Counter.builder("dang_nhap.lan_thu")
            .description("Số lần đăng nhập")
            .tag("trang_thai", "thanh_cong")
            .register(meterRegistry);
            
        this._soLanXuLyOCR = Counter.builder("ocr.xu_ly")
            .description("Số lần xử lý OCR")
            .register(meterRegistry);
            
        this._thoiGianPhanHoiAI = Timer.builder("ai.thoi_gian_phan_hoi")
            .description("Thời gian phản hồi AI")
            .register(meterRegistry);
    }
    
    public void GhiNhanDangNhap(bool thanhCong)
    {
        _soLanDangNhap.increment(Tags.of("trang_thai", thanhCong ? "thanh_cong" : "that_bai"));
    }
    
    public void GhiNhanXuLyOCR()
    {
        _soLanXuLyOCR.increment();
    }
    
    public Timer.Sample BatDauDoThoiGianAI()
    {
        return Timer.start(_thoiGianPhanHoiAI);
    }
}
```

### 13.2 Structured Logging

```csharp
// DichVuLogging.cs
public class DichVuLogging
{
    private readonly ILogger<DichVuLogging> _logger;
    
    public DichVuLogging(ILogger<DichVuLogging> logger)
    {
        _logger = logger;
    }
    
    public void GhiNhanDangNhap(string email, bool thanhCong)
    {
        _logger.LogInformation("Đăng nhập {Email} - Kết quả: {ThanhCong}", email, thanhCong);
    }
    
    public void GhiNhanTaoGiaoAn(Guid giaoVienId, string tieuDe)
    {
        _logger.LogInformation("Tạo giáo án mới - Giáo viên: {GiaoVienId}, Tiêu đề: {TieuDe}", giaoVienId, tieuDe);
    }
    
    public void GhiNhanXuLyOCR(Guid deThiId, int soBaiLam)
    {
        _logger.LogInformation("Xử lý OCR - Đề thi: {DeThiId}, Số bài làm: {SoBaiLam}", deThiId, soBaiLam);
    }
    
    public void GhiNhanLoi(Exception exception, string hanhDong)
    {
        _logger.LogError(exception, "Lỗi khi {HanhDong}", hanhDong);
    }
}
```

## 14. Performance Optimization

### 14.1 Caching Strategy

```csharp
// IDichVuCache.cs
public interface IDichVuCache
{
    Task<T> LayAsync<T>(string key);
    Task LuuAsync<T>(string key, T value, TimeSpan thoiGianHetHan);
    Task XoaAsync(string key);
}

// DichVuRedisCache.cs
public class DichVuRedisCache : IDichVuCache
{
    private readonly IDatabase _database;
    private readonly ILogger<DichVuRedisCache> _logger;
    
    public DichVuRedisCache(IConnectionMultiplexer redis, ILogger<DichVuRedisCache> logger)
    {
        _database = redis.GetDatabase();
        _logger = logger;
    }
    
    public async Task<T> LayAsync<T>(string key)
    {
        try
        {
            var value = await _database.StringGetAsync(key);
            if (value.HasValue)
            {
                return JsonSerializer.Deserialize<T>(value);
            }
            return default(T);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi lấy cache cho key: {Key}", key);
            return default(T);
        }
    }
    
    public async Task LuuAsync<T>(string key, T value, TimeSpan thoiGianHetHan)
    {
        try
        {
            var jsonValue = JsonSerializer.Serialize(value);
            await _database.StringSetAsync(key, jsonValue, thoiGianHetHan);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi lưu cache cho key: {Key}", key);
        }
    }
    
    public async Task XoaAsync(string key)
    {
        try
        {
            await _database.KeyDeleteAsync(key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi xóa cache cho key: {Key}", key);
        }
    }
}
```

### 14.2 Database Optimization

```csharp
// DichVuToiUuHoaDatabase.cs
public class DichVuToiUuHoaDatabase
{
    private readonly GiaoDucDbContext _context;
    private readonly ILogger<DichVuToiUuHoaDatabase> _logger;
    
    public DichVuToiUuHoaDatabase(GiaoDucDbContext context, ILogger<DichVuToiUuHoaDatabase> logger)
    {
        _context = context;
        _logger = logger;
    }
    
    public async Task<List<CauHoi>> LayCauHoiTheoBoLoc(BoLocCauHoi boLoc)
    {
        var query = _context.DanhSachCauHoi.AsQueryable();
        
        if (!string.IsNullOrEmpty(boLoc.MonHoc))
        {
            query = query.Where(c => c.MonHoc == boLoc.MonHoc);
        }
        
        if (!string.IsNullOrEmpty(boLoc.ChuDe))
        {
            query = query.Where(c => c.ChuDe == boLoc.ChuDe);
        }
        
        if (boLoc.MucDoKho.HasValue)
        {
            query = query.Where(c => c.MucDoKho == boLoc.MucDoKho.Value);
        }
        
        if (boLoc.Loai.HasValue)
        {
            query = query.Where(c => c.Loai == boLoc.Loai.Value);
        }
        
        // Sử dụng index cho hiệu suất
        return await query
            .Include(c => c.DanhSachLuaChon)
            .OrderByDescending(c => c.ThoiGianTao)
            .Skip((boLoc.Trang - 1) * boLoc.KichThuocTrang)
            .Take(boLoc.KichThuocTrang)
            .ToListAsync();
    }
    
    public async Task<KetQuaPhanTrang<CauHoi>> LayCauHoiPhanTrang(BoLocCauHoi boLoc)
    {
        var query = _context.DanhSachCauHoi.AsQueryable();
        
        // Áp dụng bộ lọc
        query = ApDungBoLoc(query, boLoc);
        
        var tongSo = await query.CountAsync();
        
        var danhSach = await query
            .Include(c => c.DanhSachLuaChon)
            .OrderByDescending(c => c.ThoiGianTao)
            .Skip((boLoc.Trang - 1) * boLoc.KichThuocTrang)
            .Take(boLoc.KichThuocTrang)
            .ToListAsync();
        
        return new KetQuaPhanTrang<CauHoi>
        {
            DanhSach = danhSach,
            TongSo = tongSo,
            TrangHienTai = boLoc.Trang,
            KichThuocTrang = boLoc.KichThuocTrang
        };
    }
    
    private IQueryable<CauHoi> ApDungBoLoc(IQueryable<CauHoi> query, BoLocCauHoi boLoc)
    {
        if (!string.IsNullOrEmpty(boLoc.MonHoc))
        {
            query = query.Where(c => c.MonHoc == boLoc.MonHoc);
        }
        
        if (!string.IsNullOrEmpty(boLoc.ChuDe))
        {
            query = query.Where(c => c.ChuDe == boLoc.ChuDe);
        }
        
        if (boLoc.MucDoKho.HasValue)
        {
            query = query.Where(c => c.MucDoKho == boLoc.MucDoKho.Value);
        }
        
        if (boLoc.Loai.HasValue)
        {
            query = query.Where(c => c.Loai == boLoc.Loai.Value);
        }
        
        return query;
    }
}
```

## 15. Kết luận

### 15.1 Tóm tắt thiết kế

Tài liệu DDD này đã trình bày chi tiết về:

1. **Kiến trúc Microservices** với 4 services chính và API Gateway
2. **Thiết kế Database** với các entity được tối ưu hóa cho hiệu suất
3. **API Design** tuân thủ chuẩn RESTful với đầy đủ endpoints
4. **Security Implementation** với JWT, RBAC và data encryption
5. **AI Integration** với Gemini AI và Google Vision OCR
6. **Performance Optimization** với caching và database optimization
7. **Testing Strategy** bao gồm unit, integration và E2E testing

### 15.2 Điểm mạnh của thiết kế

- **Scalability**: Kiến trúc microservices cho phép scale từng service độc lập
- **Security**: Bảo mật đa lớp với JWT, RBAC và mã hóa dữ liệu
- **Performance**: Database indexing và caching strategy được tối ưu
- **Maintainability**: Code structure rõ ràng với Vietnamese naming convention
- **User Experience**: Responsive design và OCR automation

### 15.3 Rủi ro và giảm thiểu

| Rủi ro | Mức độ | Giải pháp |
|--------|--------|-----------|
| AI service downtime | Cao | Implement circuit breaker và fallback |
| OCR accuracy issues | Trung bình | Manual correction interface |
| Database performance | Trung bình | Connection pooling và indexing |
| Security vulnerabilities | Cao | Regular security audits và updates |

### 15.4 Roadmap phát triển

**Phase 1 (Sprint 4-6)**: Core functionality
- Authentication và user management
- Basic lesson plan creation
- Question bank management

**Phase 2 (Future)**: Advanced features
- AI-powered content generation
- OCR grading system
- Analytics và reporting

**Phase 3 (Future)**: Scale và optimize
- Performance optimization
- Mobile app development
- Multi-subject support

---

*Tài liệu này sẽ được cập nhật theo tiến độ phát triển dự án.* 
