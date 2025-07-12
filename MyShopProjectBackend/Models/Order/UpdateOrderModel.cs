using MyShopProjectBackend.DTO;

namespace MyShopProjectBackend.Models.Order
{
    public class UpdateOrderModel
    {
        public int OrderId { get; set; }
        public string Status { get; set; }
    }
}
