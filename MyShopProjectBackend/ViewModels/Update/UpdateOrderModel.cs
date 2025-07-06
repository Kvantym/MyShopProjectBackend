using MyShopProjectBackend.DTO;

namespace MyShopProjectBackend.ViewModels.Update
{
    public class UpdateOrderModel
    {
        public int OrderId { get; set; }
        public string Status { get; set; }
        public string SellerId { get; set; }
    }
}
