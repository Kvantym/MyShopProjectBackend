using MyShopProjectBackend.Domain.Enums;

namespace MyShopProjectBackend.Domain.Request.Order
{
    public class UpdateOrderRequest
    {
        public int OrderId { get; set; }
        public ShopOrderStatus Status { get; set; }
    }
}
