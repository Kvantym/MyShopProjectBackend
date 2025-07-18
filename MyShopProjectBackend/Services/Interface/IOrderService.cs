using MyShopProjectBackend.DTO;
using MyShopProjectBackend.Entities;
using MyShopProjectBackend.Models.Order;

namespace MyShopProjectBackend.Servises.Interface
{
    public interface IOrderService
    {
        public Task<List<OrderDto>> GetOrdersForUserAsync(string buyerId, string buyerName);
        public Task<OrderDto?> GetOrderByIdAsync(int orderId, string sellerId);
        public Task UpdateOrderStatusAsync(UpdateOrderModel model, string sellerId);
        public Task CreateOrderAsync(string userId, List<OrderItemDto> orderItems);
        public Task DeleteOrderAsync(int orderId, string sellerId);
        public Task<List<OrderDto>> GetAllOrdersAsync(int shopId, string sellerId);
        public Task<Order> GetOrderWithDetailsAsync(int orderId);
        public Task<List<Order>> GetOrdersByBuyerAndSellerAsync(ApplicationUser buyer, ApplicationUser seller);
        public Task<List<Order>> GetOrdersByShopIdAsync(ApplicationUser seller, Shop shop);
    }
}
