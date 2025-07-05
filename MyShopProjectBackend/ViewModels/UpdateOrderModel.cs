using MyShopProjectBackend.DTO;

namespace MyShopProjectBackend.ViewModels
{
    public class UpdateOrderModel
    {
        public int OrderId { get; set; }
        public string Status { get; set; }
        public int SellerId { get; set; }
    }
}
