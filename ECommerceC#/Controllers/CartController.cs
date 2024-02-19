using ECommerceC_.Data;
using ECommerceC_.ViewModels;
using Microsoft.AspNetCore.Mvc;
using ECommerceC_.Helpers;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ECommerceC_.Controllers
{
    public class CartController : Controller
    {
        private readonly Hshop2023Context db;

        public CartController(Hshop2023Context context)
        {
            db = context;
        }
        public List<CartItem> GetCart()
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(MyConst.USER_ID);
                var cart = HttpContext.Session.Get<List<CartItem>>(MyConst.CART_KEY + userId) ?? new List<CartItem>();
                return cart;
            }
            else
            {
                var cart = HttpContext.Session.Get<List<CartItem>>(MyConst.CART_KEY) ?? new List<CartItem>();
                return cart;
            }
        }
        public List<ChiTietHd> GetInvoice()
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(MyConst.USER_ID);
                var invoiceDetails = HttpContext.Session.Get<List<ChiTietHd>>(MyConst.INVOICE_DETAILS + userId) ?? new List<ChiTietHd>();
                return invoiceDetails;
            }
            else
            {
                var invoiceDetails = HttpContext.Session.Get<List<ChiTietHd>>(MyConst.INVOICE_DETAILS) ?? new List<ChiTietHd>();
                return invoiceDetails;
            }
        }
        public IActionResult Index()
        {
            var Cart = GetCart();
            return View(Cart);
        }

        public IActionResult AddToCart(int id, int quantity = 1)
        {
            var userId = "";
            if (User.Identity.IsAuthenticated)
            {
                userId = User.FindFirstValue(MyConst.USER_ID);
                
            }
            var cart = GetCart();
            var item = cart.SingleOrDefault(p => p.ProductId == id);
            if (item == null) {
                var product = db.HangHoas.SingleOrDefault(p => p.MaHh == id);
                if (product == null)
                {
                    TempData["Message"] = $"The product not exist";
                    return Redirect("/404");
                }
                item = new CartItem
                {
                    ProductId = product.MaHh,
                    ProductName = product.TenHh,
                    ProductPrice = product.DonGia ?? 0,
                    ProductImg = product.Hinh ?? string.Empty,
                    ProductQuantity = quantity
                };
                cart.Add(item);
            }
            else
            {
                item.ProductQuantity += quantity;
            }
            if (userId == "")
            {
                HttpContext.Session.Set(MyConst.CART_KEY, cart);
            }        
            HttpContext.Session.Set(MyConst.CART_KEY + userId, cart);
            return RedirectToAction("Index");
        }

        public IActionResult RemoveCart(int id)
        {
            var userId = "";
            if (User.Identity.IsAuthenticated)
            {
                userId = User.FindFirstValue(MyConst.USER_ID);

            }
            var cart = GetCart();
            var item = cart.SingleOrDefault(p => p.ProductId == id);
            if (item != null)
            {
                cart.Remove(item);
                if (userId == "")
                {
                    HttpContext.Session.Set(MyConst.CART_KEY, cart);
                }
                else
                {
                    HttpContext.Session.Set(MyConst.CART_KEY + userId, cart);
                }          
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult UpdateQuantity (int id, int quantity)
        {
            var userId = "";
            if (User.Identity.IsAuthenticated)
            {
                userId = User.FindFirstValue(MyConst.USER_ID);
            }
            var cart = GetCart();
            var product = cart.FirstOrDefault(p => p.ProductId == id);
            if (product != null)
            {
                product.ProductQuantity = quantity;
                if (userId == "")
                {
                    HttpContext.Session.Set(MyConst.CART_KEY, cart);
                }
                else
                {
                    HttpContext.Session.Set(MyConst.CART_KEY + userId, cart);
                }
            }
            
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Checkout()
        {
            var userId = "";
            var cart = GetCart();
            if (User.Identity.IsAuthenticated)
            {
                userId = User.FindFirstValue(MyConst.USER_ID);
                var customer = db.KhachHangs.SingleOrDefault(kh => kh.MaKh == userId);
                ViewData["CustomerEmail"] = customer.Email;
                ViewData["CustomerNumber"] = customer.DienThoai;
                ViewData["CustomerAddress"] = customer.DiaChi;
            }
            if (cart.Count == 0 )
            {
                return Redirect("/");
            }
            return View(cart);
        }

        [HttpPost]
        public IActionResult Checkout(CheckoutVM model)
        {
            var userId = "";
            if (ModelState.IsValid)
            {
                if (User.Identity.IsAuthenticated)
                {
                    userId = User.FindFirstValue(MyConst.USER_ID);
                }
                var invoice = new HoaDon
                {
                    MaKh = userId ?? "UKNOW",
                    HoTen = model.CustomerName,
                    DiaChi = model.CustomerAddress,
                    DienThoai = model.CustomerNumber,
                    NgayDat = DateTime.Now,
                    CachThanhToan = "COD",
                    CachVanChuyen = "GRAB",
                    MaTrangThai = 0,
                    GhiChu = model.OrderNote
                };
                db.Database.BeginTransaction();
                try
                {
                    db.Database.CommitTransaction();
                    db.Add(invoice);
                    db.SaveChanges();

                    var cart = GetCart();
                    var invoiceDetails = GetInvoice();
                    foreach (var item in cart)
                    {   
                        invoiceDetails.Add(new ChiTietHd
                        {
                            MaHd = invoice.MaHd,
                            SoLuong = item.ProductQuantity,
                            DonGia = item.ProductPrice,
                            MaHh = item.ProductId,
                            GiamGia = 0
                        });                    
                    }
                    if (userId != "")
                    {
                        HttpContext.Session.Set(MyConst.INVOICE_DETAILS + userId, invoiceDetails);
                    }
                    db.AddRange(invoiceDetails);
                    db.SaveChanges();
                    if (userId == "")
                    {
                        HttpContext.Session.Set(MyConst.CART_KEY, new List<CartItem>());
                    }
                    
                    return View("Success");
                }
                catch
                {
                    db.Database.RollbackTransaction();
                }
            }
            return View();
        }
    }
}
