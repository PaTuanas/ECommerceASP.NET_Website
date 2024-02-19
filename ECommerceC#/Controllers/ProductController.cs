using AutoMapper;
using ECommerceC_.Data;
using ECommerceC_.Helpers;
using ECommerceC_.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ECommerceC_.Controllers
{
    public class ProductController : Controller
    {
        private readonly Hshop2023Context db;
        private readonly IMapper _mapper;

        public ProductController(Hshop2023Context context, IMapper mapper) {
            db = context;
            _mapper = mapper;
        }
        public IActionResult Index(int? categoryId)
        {
            var Products = db.HangHoas.AsQueryable();

            if (categoryId.HasValue)
            {
                Products = Products.Where(p => p.MaLoai == categoryId.Value);
            }
            var result = Products.Select(p => new ProductVM {
                ProductId = p.MaHh,
                ProductName = p.TenHh,
                ProductPrice = p.DonGia ?? 0,
                ProductImg = p.Hinh ?? "",
                ProductDescription = p.MoTaDonVi ?? "",
                ProductType = p.MaLoaiNavigation.TenLoai
            });
            return View(result);
        }

        public IActionResult Search(string query)
        {
            var Products = db.HangHoas.AsQueryable();

            if (query != null)
            {
                Products = Products.Where(p => p.TenHh.Contains(query));
            }
            var result = Products.Select(p => new ProductVM
            {
                ProductId = p.MaHh,
                ProductName = p.TenHh,
                ProductPrice = p.DonGia ?? 0,
                ProductImg = p.Hinh ?? "",
                ProductDescription = p.MoTaDonVi ?? "",
                ProductType = p.MaLoaiNavigation.TenLoai
            });
            return View(result);
        }

        public IActionResult Detail(int id)
        {
            var data = db.HangHoas.Include(p => p.MaLoaiNavigation).SingleOrDefault(p => p.MaHh == id);
            if (data == null)
            {
                TempData["Message"] = $"The product not exists";
                return Redirect("/404");
            }

            var result = new ProductDetailVM
            {
                ProductId = data.MaHh,
                ProductName = data.TenHh,
                ProductPrice = data.DonGia ?? 0,
                ProductDetail = data.MoTa ?? string.Empty,
                ProductRating = 5,
                ProductImg = data.Hinh ?? string.Empty,
                ProductDescription = data.MoTaDonVi ?? string.Empty,
                ProductType = data.MaLoaiNavigation.TenLoai,
                ProductQuantity = 10
            };
            return View(result);
        }


        [HttpGet]
        public IActionResult Create()
        { 
            return View();
        }

        [HttpPost]
        public IActionResult Create(ProductVM model, IFormFile? img)
        {
            if (ModelState.IsValid)
            {
               try
                {
                    var category = db.Loais.FirstOrDefault(lo => lo.TenLoai == model.ProductType);
                    var supplier = db.NhaCungCaps.FirstOrDefault(ncc => ncc.TenCongTy == model.Supplier);
                    var product = _mapper.Map<HangHoa>(model);
                    product.MaLoai = category.MaLoai;
                    product.MaNcc = supplier.MaNcc;
                    if (img != null)
                    {
                        product.Hinh = MyUtil.UploadImage(img, "HangHoa");
                    }
                    db.Add(product);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch { 
                }
            }
            return View();
        }
		[Route("Edit")]
		[HttpGet]
		public IActionResult Edit(int id)
        {
			try
            {
				var data = db.HangHoas.Include(p => p.MaLoaiNavigation).Include(p => p.MaNccNavigation).SingleOrDefault(hh => hh.MaHh == id);
				if (data != null)
				{
					var result = new ProductDetailVM
					{
						ProductId = data.MaHh,
						ProductName = data.TenHh,
						ProductPrice = data.DonGia ?? 0,
						ProductDetail = data.MoTaDonVi ?? string.Empty,
						ProductRating = 5,
						ProductImg = data.Hinh ?? string.Empty,
                        Supplier = data.MaNccNavigation.TenCongTy ?? string.Empty,
                        ProductDescription = data.MoTa ?? string.Empty,
						ProductType = data.MaLoaiNavigation.TenLoai ?? string.Empty,
						ProductQuantity = 10
					};
					return View(result);
				}
			}
            catch (Exception ex)
            {
				
			}
			return View();
		}
        [Route("Edit")]
        [HttpPost]
        public IActionResult Edit(ProductDetailVM model, IFormFile? img)
        {
			try
			{
				var product = _mapper.Map<ProductDetailVM>(model);
				if (img != null)
				{
					if (!string.IsNullOrEmpty(product.ProductImg))
					{
						var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Hinh", "HangHoa", product.ProductImg);
						if (System.IO.File.Exists(oldImagePath))
						{
							System.IO.File.Delete(oldImagePath);
						}
					}

					product.ProductImg = MyUtil.UploadImage(img, "HangHoa");
					 
				}
				
				var existingProduct = db.HangHoas.Include(p => p.MaLoaiNavigation).Include(p => p.MaNccNavigation).FirstOrDefault(p => p.MaHh == product.ProductId);

				if (existingProduct != null)
				{
					existingProduct.MaHh = product.ProductId;
					existingProduct.TenHh = product.ProductName;
					existingProduct.DonGia = product.ProductPrice;
					existingProduct.MoTa = product.ProductDescription;
                    existingProduct.MoTaDonVi = product.ProductDetail;
					existingProduct.MaLoaiNavigation.TenLoai = product.ProductType;
					existingProduct.MaNccNavigation.TenCongTy = product.Supplier;
                    existingProduct.Hinh = product.ProductImg;
				}
				
				db.Entry(existingProduct).State = EntityState.Modified;
				db.SaveChanges();
				return View(model);
			}
			catch
			{

			}
			return View(model);
		}

        [Route("Delete")]
        [HttpGet]
        public IActionResult Delete(int id)
        {
            try
            {
                var data = db.ChiTietHds.SingleOrDefault(p => p.MaHh == id);
                if (data == null)
                {
                    var product = db.HangHoas.FirstOrDefault(p => p.MaHh == id);
                    if (product.Hinh != null)
                    {
                        var imgPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Hinh", "HangHoa", product.Hinh);
                        if (System.IO.File.Exists(imgPath))
                        {
                            System.IO.File.Delete(imgPath);
                        }
                    }
					db.Remove(product);
					db.SaveChanges();
					TempData["Message"] = "Delete product successfully";
					return RedirectToAction("Index");
				}
				TempData["Message"] = "Cannot delete this product!";
				return RedirectToAction("Index");
            }
            catch
            {

            }
			return RedirectToAction(null);

		}
    }

}
