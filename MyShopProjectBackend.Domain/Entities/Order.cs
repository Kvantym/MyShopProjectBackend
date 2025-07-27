using MyShopProjectBackend.Domain.Enums;

namespace MyShopProjectBackend.Domain.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.UtcNow;
        public ApplicationUser Buyer { get; set; }
        public ApplicationUser Seller { get; set; }
        public string BuyerId { get; set; }
        public string SellerId { get; set; }
        public int ShopId { get; set; }
        public ShopOrderStatus Status { get; set; }
        public Shop Shop { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public ICollection<ShopOrder> ShopOrders { get; set; } = new List<ShopOrder>();
    }
}
