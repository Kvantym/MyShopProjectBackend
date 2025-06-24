namespace MyShopProjectBackend.ViewModels
{
    public class CreateProductModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }
        public int ShopId { get; set; }
    }
}
