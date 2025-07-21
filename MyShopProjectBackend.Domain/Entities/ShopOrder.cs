namespace MyShopProjectBackend.Domain.Entities
{
    public class ShopOrder
    {
        public int Id { get; set; }
        public int ShopId { get; set; }
        public Shop Shop { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }
        public string Status { get; set; }
    }
}
