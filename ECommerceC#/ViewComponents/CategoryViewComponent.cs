using ECommerceC_.Data;
using ECommerceC_.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceC_.ViewComponents
{
    public class CategoryViewComponent : ViewComponent
    {
        private readonly Hshop2023Context db;

        public CategoryViewComponent(Hshop2023Context context) => db = context;

        public  IViewComponentResult Invoke()
        {
            var data = db.Loais.Select(lo => new CategoryVM
            {
                IdProduct = lo.MaLoai,
                ProductName = lo.TenLoai,
                Quantity = lo.HangHoas.Count
            }).OrderBy(p => p.ProductName);

            return View(data);
        }
    }
}
