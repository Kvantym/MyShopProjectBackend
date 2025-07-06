using MyShopProjectBackend.DTO;
using MyShopProjectBackend.ViewModels.Update;

namespace MyShopProjectBackend.Servises.Interface
{
    public interface IOrderServises
    {
        public Task<List<OrderDto>> GetOrdersForUserAsync(string buyerId, string buyerName);
        public Task<OrderDto?> GetOrderByIdAsync(int orderId, string sellerId);
        public Task UpdateOrderStatusAsync(UpdateOrderModel model);
        public Task CreateOrderAsync(string userId, List<OrderItemDto> orderItems);
        public Task DeleteOrderAsync(int orderId, string sellerId);
        public Task<List<OrderDto>> GetAllOrdersAsync(int shopId, string sellerId);
    }
}
