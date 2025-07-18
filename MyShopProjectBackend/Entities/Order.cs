namespace MyShopProjectBackend.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime  CreateAt { get; set; } = DateTime.UtcNow;
        public string BuyerId { get; set; }
        public string Status { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public ICollection<ShopOrder> ShopOrders { get; set; } = new List<ShopOrder>();
    }
}
