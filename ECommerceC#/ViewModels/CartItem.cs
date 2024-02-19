namespace ECommerceC_.ViewModels
{
    public class CartItem
    {
        public int ProductId { get; set; }
        public string ProductImg { get; set; }
        public string ProductName { get; set; }
        public double ProductPrice { get; set; }    
        public int ProductQuantity { get; set; }

        public double Total => ProductPrice * ProductQuantity;
    }
}
