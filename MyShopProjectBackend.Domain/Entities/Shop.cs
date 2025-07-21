namespace MyShopProjectBackend.Domain.Entities
{
    public class Shop
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string OwnerId { get; set; }
        public ICollection<Product> Products { get; set; } = new List<Product>();
        public ICollection<ShopOrder> Orders { get; set; } = new List<ShopOrder>();
    }
}
