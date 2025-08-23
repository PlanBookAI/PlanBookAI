using ExamService.Interfaces;
using ExamService.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ExamService.Services
{
    public class ThongKeService : IThongKeService
    {
        private readonly ICauHoiRepository _cauHoiRepo;
        private readonly IDeThiRepository _deThiRepo;
        private readonly IMauDeThiRepository _mauDeThiRepo;

        public ThongKeService(ICauHoiRepository cauHoiRepo, IDeThiRepository deThiRepo, IMauDeThiRepository mauDeThiRepo)
        {
            _cauHoiRepo = cauHoiRepo;
            _deThiRepo = deThiRepo;
            _mauDeThiRepo = mauDeThiRepo;
        }

        public async Task<ApiPhanHoi<ThongKeTongQuanDTO>> GetGeneralStatisticsAsync(Guid teacherId)
        {
            // Lấy IQueryable để xây dựng truy vấn
            var questionQuery = _cauHoiRepo.GetQueryable().Where(q => q.NguoiTaoId == teacherId);
            var examQuery = _deThiRepo.GetQueryable().Where(d => d.NguoiTaoId == teacherId);

            // Thực thi tất cả các truy vấn bất đồng bộ song song để tối ưu thời gian
            var tongSoCauHoiTask = questionQuery.CountAsync();
            var tongSoDeThiTask = examQuery.CountAsync();

            var thongKeMonHocTask = questionQuery
                .GroupBy(q => q.MonHoc)
                .Select(g => new ThongKeTheoNhomDTO { TenNhom = g.Key, SoLuong = g.Count() })
                .ToListAsync();

            var thongKeDoKhoTask = questionQuery
                .GroupBy(q => q.DoKho)
                .Select(g => new ThongKeTheoNhomDTO { TenNhom = g.Key, SoLuong = g.Count() })
                .ToListAsync();

            var thongKeChuDeTask = questionQuery
                .Where(q => q.ChuDe != null)
                .GroupBy(q => q.ChuDe)
                .Select(g => new ThongKeTheoNhomDTO { TenNhom = g.Key!, SoLuong = g.Count() })
                .ToListAsync();

            // Chờ tất cả các task hoàn thành
            await Task.WhenAll(tongSoCauHoiTask, tongSoDeThiTask, thongKeMonHocTask, thongKeDoKhoTask, thongKeChuDeTask);

            // Tổng hợp kết quả
            var result = new ThongKeTongQuanDTO
            {
                TongSoCauHoi = await tongSoCauHoiTask,
                TongSoDeThi = await tongSoDeThiTask,
                ThongKeTheoMonHoc = await thongKeMonHocTask,
                ThongKeTheoDoKho = await thongKeDoKhoTask,
                ThongKeTheoChuDe = await thongKeChuDeTask
            };

            return ApiPhanHoi<ThongKeTongQuanDTO>.ThanhCongVoiDuLieu(result);
        }

        public async Task<ApiPhanHoi<CauHoiThongKeTongQuanDTO>> GetQuestionStatisticsAsync(Guid teacherId)
        {
            var questionQuery = _cauHoiRepo.GetQueryable().Where(q => q.NguoiTaoId == teacherId);

            // Chạy các task truy vấn song song
            var tongSoCauHoiTask = questionQuery.CountAsync();

            var thongKeMonHocTask = questionQuery
                .GroupBy(q => q.MonHoc)
                .Select(g => new ThongKeTheoNhomDTO { TenNhom = g.Key, SoLuong = g.Count() })
                .ToListAsync();

            var thongKeDoKhoTask = questionQuery
                .GroupBy(q => q.DoKho)
                .Select(g => new ThongKeTheoNhomDTO { TenNhom = g.Key, SoLuong = g.Count() })
                .ToListAsync();

            var thongKeChuDeTask = questionQuery
                .Where(q => q.ChuDe != null)
                .GroupBy(q => q.ChuDe)
                .Select(g => new ThongKeTheoNhomDTO { TenNhom = g.Key!, SoLuong = g.Count() })
                .ToListAsync();

            await Task.WhenAll(tongSoCauHoiTask, thongKeMonHocTask, thongKeDoKhoTask, thongKeChuDeTask);

            var result = new CauHoiThongKeTongQuanDTO
            {
                TongSoCauHoi = await tongSoCauHoiTask,
                ThongKeTheoMonHoc = await thongKeMonHocTask,
                ThongKeTheoDoKho = await thongKeDoKhoTask,
                ThongKeTheoChuDe = await thongKeChuDeTask
            };

            return ApiPhanHoi<CauHoiThongKeTongQuanDTO>.ThanhCongVoiDuLieu(result);
        }

        public async Task<ApiPhanHoi<DeThiThongKeTongQuanDTO>> GetExamStatisticsOverviewAsync(Guid teacherId)
        {
            var examQuery = _deThiRepo.GetQueryable().Where(d => d.NguoiTaoId == teacherId);

            // Chạy các task truy vấn song song
            var tongSoDeThiTask = examQuery.CountAsync();

            var thongKeTrangThaiTask = examQuery
                .GroupBy(d => d.TrangThai)
                .Select(g => new ThongKeTheoNhomDTO { TenNhom = g.Key, SoLuong = g.Count() })
                .ToListAsync();

            var thongKeMonHocTask = examQuery
                .GroupBy(d => d.MonHoc)
                .Select(g => new ThongKeTheoNhomDTO { TenNhom = g.Key, SoLuong = g.Count() })
                .ToListAsync();

            var thongKeKhoiLopTask = examQuery
                .GroupBy(d => d.KhoiLop)
                .Select(g => new ThongKeTheoNhomDTO { TenNhom = g.Key.ToString(), SoLuong = g.Count() })
                .ToListAsync();

            await Task.WhenAll(tongSoDeThiTask, thongKeTrangThaiTask, thongKeMonHocTask, thongKeKhoiLopTask);

            var result = new DeThiThongKeTongQuanDTO
            {
                TongSoDeThi = await tongSoDeThiTask,
                ThongKeTheoTrangThai = await thongKeTrangThaiTask,
                ThongKeTheoMonHoc = await thongKeMonHocTask,
                ThongKeTheoKhoiLop = await thongKeKhoiLopTask
            };

            return ApiPhanHoi<DeThiThongKeTongQuanDTO>.ThanhCongVoiDuLieu(result);
        }

        public async Task<ApiPhanHoi<List<ThongKeTheoNhomDTO>>> GetStatsByDifficultyAsync(Guid teacherId)
        {
            var stats = await _cauHoiRepo.GetQueryable()
                .Where(q => q.NguoiTaoId == teacherId)
                .GroupBy(q => q.DoKho)
                .Select(g => new ThongKeTheoNhomDTO { TenNhom = g.Key, SoLuong = g.Count() })
                .AsNoTracking()
                .ToListAsync();

            return ApiPhanHoi<List<ThongKeTheoNhomDTO>>.ThanhCongVoiDuLieu(stats);
        }

        public async Task<ApiPhanHoi<List<ThongKeTheoNhomDTO>>> GetStatsByTopicAsync(Guid teacherId)
        {
            var stats = await _cauHoiRepo.GetQueryable()
                .Where(q => q.NguoiTaoId == teacherId && q.ChuDe != null) // Chỉ lấy các câu hỏi có chủ đề
                .GroupBy(q => q.ChuDe)
                .Select(g => new ThongKeTheoNhomDTO { TenNhom = g.Key!, SoLuong = g.Count() })
                .OrderByDescending(s => s.SoLuong) // Sắp xếp theo chủ đề có nhiều câu hỏi nhất
                .AsNoTracking()
                .ToListAsync();

            return ApiPhanHoi<List<ThongKeTheoNhomDTO>>.ThanhCongVoiDuLieu(stats);
        }

        public async Task<ApiPhanHoi<List<ThongKeTheoNhomDTO>>> GetStatsBySubjectAsync(Guid teacherId)
        {
            var stats = await _cauHoiRepo.GetQueryable()
                .Where(q => q.NguoiTaoId == teacherId)
                .GroupBy(q => q.MonHoc)
                .Select(g => new ThongKeTheoNhomDTO { TenNhom = g.Key, SoLuong = g.Count() })
                .OrderByDescending(s => s.SoLuong) // Sắp xếp theo môn học có nhiều câu hỏi nhất
                .AsNoTracking()
                .ToListAsync();

            return ApiPhanHoi<List<ThongKeTheoNhomDTO>>.ThanhCongVoiDuLieu(stats);
        }

        public async Task<ApiPhanHoi<NguoiDungThongKeDTO>> GetUserStatisticsAsync(Guid teacherId)
        {
            var questionCountTask = _cauHoiRepo.GetQueryable()
                .CountAsync(q => q.NguoiTaoId == teacherId);

            var examCountTask = _deThiRepo.GetQueryable()
                .CountAsync(d => d.NguoiTaoId == teacherId);

            var templateCountTask = _mauDeThiRepo.GetQueryable()
                .CountAsync(m => m.NguoiTaoId == teacherId);

            await Task.WhenAll(questionCountTask, examCountTask, templateCountTask);

            var stats = new NguoiDungThongKeDTO
            {
                UserId = teacherId,
                TongSoCauHoiDaTao = await questionCountTask,
                TongSoDeThiDaTao = await examCountTask,
                TongSoMauDeThiDaTao = await templateCountTask
            };

            return ApiPhanHoi<NguoiDungThongKeDTO>.ThanhCongVoiDuLieu(stats);
        }
    }
}