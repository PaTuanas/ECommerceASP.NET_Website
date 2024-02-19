using AutoMapper;
using ECommerceC_.Data;
using ECommerceC_.Helpers;
using ECommerceC_.ViewModels;
using ECommerceMVC.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerceC_.Controllers
{
    public class CustomerController : Controller
    {
        private readonly Hshop2023Context db;
        private readonly IMapper _mapper;

        public CustomerController(Hshop2023Context context, IMapper mapper) {
            db = context;
            _mapper = mapper;
        }

		#region Register
		[HttpGet]
        public IActionResult Register()
        {
            return View();
        }

		[HttpPost]
		public IActionResult Register(RegisterVM model, IFormFile? Img)
		{
			if (ModelState.IsValid)
            {
               
                try
                {
                    var customer = _mapper.Map<KhachHang>(model);
                    customer.RandomKey = MyUtil.GenerateRandomKey();
                    customer.MatKhau = model.Password.ToMd5Hash(customer.RandomKey);
                    customer.HieuLuc = true;
                    customer.VaiTro = 0;

                    if (Img != null)
                    {
                        customer.Hinh = MyUtil.UploadImage(Img, "KhachHang");
                    }
                    db.Add(customer);
                    db.SaveChanges();
                    return RedirectToAction("Login", "Customer");
                }
                catch (Exception ex)
                {
                    var mess = $"{ex.Message} shh";
                }
            }
			return View();
		}
        #endregion
        #region Login
        [HttpGet]
        public IActionResult Login(string? ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login (LoginVM model, string? ReturnUrl)
        {
            try
            {
                ViewBag.ReturnUrl = ReturnUrl;
                if (ModelState.IsValid)
                {
                    var user = db.KhachHangs.SingleOrDefault(kh => kh.MaKh == model.UserName);
                    if (user == null)
                    {
                        ModelState.AddModelError("Error", "Account does not existed");
                    }
                    else
                    {
                        if (!user.HieuLuc)
                        {
                            ModelState.AddModelError("Error", "Account has been blocked");
                        }
                        else
                        {
                            if (user.MatKhau != model.Password.ToMd5Hash(user.RandomKey))
                            {
                                ModelState.AddModelError("Error", "Username or password is incorrect");
                            }
                            else
                            {
                                if(!HttpContext.Session.Keys.Contains("UserID"))
                                {
                                    HttpContext.Session.SetString("UserID", user.MaKh);
                                    var cart = new List<CartItem>();
                                    HttpContext.Session.Set(MyConst.CART_KEY + user.MaKh, cart);
                                    var invoiceDetails = new List<ChiTietHd>();
                                    HttpContext.Session.Set(MyConst.INVOICE_DETAILS + user.MaKh, invoiceDetails);
                                }
                                else
                                {
                                    var userId = HttpContext.Session.GetString("UserID");
                                    var cart = HttpContext.Session.Get<List<CartItem>>(MyConst.CART_KEY + userId);
                                    var invoiceDetails = HttpContext.Session.Get<List<ChiTietHd>>(MyConst.INVOICE_DETAILS + userId);

                                }
                                var userRole = "";
                                if (user.VaiTro == 1)
                                {
                                    userRole = "Admin";     
                                }
                                else
                                {
                                    userRole = "Customer";    
                                }
                                var claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.Email, user.Email),
                                new Claim(ClaimTypes.Name, user.HoTen),
                                new Claim(ClaimTypes.MobilePhone, user.DienThoai),
                                new Claim(ClaimTypes.StreetAddress, user.DiaChi),
                                new Claim("UserID", user.MaKh),
                                new Claim(ClaimTypes.Role, userRole)
                            };
                                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

								bool isInCustomerRole = claimsPrincipal.IsInRole("Customer");
								bool isInAdminRole = claimsPrincipal.IsInRole("Admin");

								await HttpContext.SignInAsync(claimsPrincipal);

                                if (Url.IsLocalUrl(ReturnUrl))
                                {
                                    return Redirect(ReturnUrl);
                                }
                                else
                                {
                                    return Redirect("/");
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return View();
        }
        #endregion

        [Authorize]
        public IActionResult Profile()
        {
			string userEmail = User.FindFirstValue(ClaimTypes.Email);
			string userName = User.FindFirstValue(ClaimTypes.Name);
			string userId = User.FindFirstValue("UserID");

			// Ví dụ hiển thị thông tin người dùng
			ViewData["UserEmail"] = userEmail;
			ViewData["UserName"] = userName;
			ViewData["UserId"] = userId;

			return View();
        }

		[Authorize]
		public async Task<IActionResult> Logout()
		{
			await HttpContext.SignOutAsync();
			return Redirect("/");
		}
	}

}
