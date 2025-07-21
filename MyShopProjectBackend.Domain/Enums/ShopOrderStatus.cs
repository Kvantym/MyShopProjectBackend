

namespace MyShopProjectBackend.Domain.Enums
{
    public enum ShopOrderStatus
    {
        Pending = 0,
        Failed = 1,
        Completed = 2,
        Cancelled = 3,
        InProgress = 4,
        Refunded = 5,
        Shipped = 6,
        Delivered = 7,
        Confirmed = 8,
        Shipping = 9
    }
}
