using AutoMapper;
using ECommerceC_.Data;
using ECommerceC_.ViewModels;

namespace ECommerceC_.Helpers
{
	public class AutoMapperProfile : Profile
	{
		public AutoMapperProfile()
		{
			CreateMap<RegisterVM, KhachHang>()
				.ForMember(kh => kh.MaKh, option => option.MapFrom(RegisterVM => RegisterVM.UserId))
				.ForMember(kh => kh.DienThoai, option => option.MapFrom(RegisterVM => RegisterVM.PhoneNumber))
				.ForMember(kh => kh.NgaySinh, option => option.MapFrom(RegisterVM => RegisterVM.DateOfBirth))
				.ForMember(kh => kh.GioiTinh, option => option.MapFrom(RegisterVM => RegisterVM.Gender))
				.ForMember(kh => kh.Email, option => option.MapFrom(RegisterVM => RegisterVM.Email))
				.ForMember(kh => kh.DiaChi, option => option.MapFrom(RegisterVM => RegisterVM.Address))
				.ForMember(kh => kh.HoTen, option => option.MapFrom(RegisterVM => RegisterVM.FullName));
			CreateMap<ProductVM, HangHoa>()
				.ForMember(kh => kh.MaHh, option => option.MapFrom(ProductVM => ProductVM.ProductId))
				.ForMember(kh => kh.TenHh, option => option.MapFrom(ProductVM => ProductVM.ProductName))
				.ForMember(kh => kh.DonGia, option => option.MapFrom(ProductVM => ProductVM.ProductPrice))
				.ForMember(kh => kh.MoTa, option => option.MapFrom(ProductVM => ProductVM.ProductDescription));
			CreateMap<ProductDetailVM, HangHoa>()
				.ForMember(kh => kh.MaHh, option => option.MapFrom(ProductDetailVM => ProductDetailVM.ProductId))
				.ForMember(kh => kh.TenHh, option => option.MapFrom(ProductDetailVM => ProductDetailVM.ProductName))
				.ForMember(kh => kh.DonGia, option => option.MapFrom(ProductDetailVM => ProductDetailVM.ProductPrice))
				.ForMember(kh => kh.MoTa, option => option.MapFrom(ProductDetailVM => ProductDetailVM.ProductDescription))
				.ForMember(kh => kh.MaNccNavigation, option => option.MapFrom(ProductDetailVM => ProductDetailVM.Supplier))
				.ForMember(kh => kh.MaLoaiNavigation, option => option.MapFrom(ProductDetailVM => ProductDetailVM.ProductType))
				.ForMember(kh => kh.Hinh, option => option.MapFrom(ProductDetailVM => ProductDetailVM.ProductImg));

		}
	}
}
