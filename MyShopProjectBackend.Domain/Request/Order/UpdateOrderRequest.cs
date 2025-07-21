namespace MyShopProjectBackend.Domain.Request.Order
{
    public class UpdateOrderRequest
    {
        public int OrderId { get; set; }
        public string Status { get; set; }
    }
}
