using ECommerceC_.Helpers;
using ECommerceC_.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace ECommerceC_.ViewComponents
{
    public class CartViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var cart = HttpContext.Session.Get<List<CartItem>>(MyConst.CART_KEY) ?? new List<CartItem>();
            return View("CartPanel", new CartModel
            {
                Quantity = cart.Sum(p => p.ProductQuantity),
                Total = cart.Sum(p => p.Total)
            });
        }
    }
}
