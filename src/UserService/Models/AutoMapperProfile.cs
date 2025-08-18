using AutoMapper;
using UserService.Models.DTOs;
using UserService.Models.Entities;

namespace UserService.Models;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        // Map NguoiDung to ThongTinHoSoDto
        CreateMap<NguoiDung, ThongTinHoSoDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.HoTen, opt => opt.MapFrom(src => src.HoSoNguoiDung != null ? src.HoSoNguoiDung.HoTen : ""))
            .ForMember(dest => dest.SoDienThoai, opt => opt.MapFrom(src => src.HoSoNguoiDung != null ? src.HoSoNguoiDung.SoDienThoai : null))
            .ForMember(dest => dest.DiaChi, opt => opt.MapFrom(src => src.HoSoNguoiDung != null ? src.HoSoNguoiDung.DiaChi : null))
            .ForMember(dest => dest.MoTaBanThan, opt => opt.MapFrom(src => src.HoSoNguoiDung != null ? src.HoSoNguoiDung.MoTaBanThan : null))
            .ForMember(dest => dest.AnhDaiDienUrl, opt => opt.MapFrom(src => src.HoSoNguoiDung != null ? src.HoSoNguoiDung.AnhDaiDienUrl : null))
            .ForMember(dest => dest.TaoLuc, opt => opt.MapFrom(src => src.TaoLuc))
            .ForMember(dest => dest.CapNhatLuc, opt => opt.MapFrom(src => src.CapNhatLuc));

        // Map NguoiDung to ThongTinNguoiDungDto
        CreateMap<NguoiDung, ThongTinNguoiDungDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.VaiTro, opt => opt.MapFrom(src => src.VaiTro != null ? src.VaiTro.Ten : "Unknown"))
            .ForMember(dest => dest.HoatDong, opt => opt.MapFrom(src => src.HoatDong))
            .ForMember(dest => dest.DaXoa, opt => opt.MapFrom(src => src.IsDeleted))
            .ForMember(dest => dest.NgayXoa, opt => opt.MapFrom(src => src.DeletedAt))
            .ForMember(dest => dest.TaoLuc, opt => opt.MapFrom(src => src.TaoLuc))
            .ForMember(dest => dest.CapNhatLuc, opt => opt.MapFrom(src => src.CapNhatLuc))
            .ForMember(dest => dest.HoTen, opt => opt.MapFrom(src => src.HoSoNguoiDung != null ? src.HoSoNguoiDung.HoTen : ""))
            .ForMember(dest => dest.SoDienThoai, opt => opt.MapFrom(src => src.HoSoNguoiDung != null ? src.HoSoNguoiDung.SoDienThoai : null))
            .ForMember(dest => dest.DiaChi, opt => opt.MapFrom(src => src.HoSoNguoiDung != null ? src.HoSoNguoiDung.DiaChi : null))
            .ForMember(dest => dest.AnhDaiDienUrl, opt => opt.MapFrom(src => src.HoSoNguoiDung != null ? src.HoSoNguoiDung.AnhDaiDienUrl : null));

			// Mapping LichSuDangNhap
			CreateMap<LichSuDangNhap, ThongTinLichSuDangNhapDto>()
				.ForMember(dest => dest.Email, opt => opt.MapFrom(src => "")) // Không có navigation property, để trống
				.ForMember(dest => dest.NgayDangNhap, opt => opt.MapFrom(src => src.CreatedAt));
    }
}
