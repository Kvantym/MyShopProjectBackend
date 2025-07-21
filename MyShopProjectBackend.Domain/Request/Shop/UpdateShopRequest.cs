namespace MyShopProjectBackend.Domain.Request.Shop
{
    public class UpdateShopRequest
    {
        public int ShopId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
