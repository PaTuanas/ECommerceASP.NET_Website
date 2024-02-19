using System.ComponentModel.DataAnnotations;

namespace ECommerceC_.ViewModels
{
    public class ProductVM
    {
        
        public int ProductId { get; set; }
        [Display(Name = "Product Name")]
        [Required(ErrorMessage = "*")]
        public string ProductName { get; set; }
        [Display(Name = "Product Image")]
        public string? ProductImg {  get; set; }
        [Display(Name = "Product Price")]
        [Required(ErrorMessage = "*")]
        public double ProductPrice { get; set; }
        [Display(Name = "Product Decription")]
        public string? ProductDescription {  get; set; }
        [Display(Name = "Product Type")]
        public string ProductType { get; set; }

		[Display(Name = "Supplier")]
		public string Supplier { get; set; }
    }

    public class ProductDetailVM
    {
        public int ProductId { get; set; }
		[Display(Name = "Product Name")]
		[Required(ErrorMessage = "*")]
		public string ProductName { get; set; }
		[Display(Name = "Product Image")]
		public string? ProductImg { get; set; }
		[Display(Name = "Product Price")]
		[Required(ErrorMessage = "*")]
		public double ProductPrice { get; set; }
		[Display(Name = "Product Decription")]
		public string? ProductDescription { get; set; }
		[Display(Name = "Product Type")]
		public string ProductType { get; set; }

		[Display(Name = "Supplier")]
		public string? Supplier { get; set; }
		[Display(Name = "Product Detail")]
		public string ProductDetail { get; set; }
		[Display(Name = "Product Rating")]
		public int ProductRating { get; set; }
		[Display(Name = "Product Quantity")]
		public int ProductQuantity { get; set; }

    }
}
