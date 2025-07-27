using MyShopProjectBackend.Domain.Entities;
using MyShopProjectBackend.Domain.Request.Order;
using MyShopProjectBackend.Domain.Responses;

namespace MyShopProjectBackend.Services.Interface
{
    public interface IOrderService
    {
        public Task<List<OrderResponse>> GetOrdersForUserAsync(string buyerName);
        public Task<OrderResponse?> GetOrderByIdAsync(int orderId, string sellerId);
        public Task UpdateOrderStatusAsync(UpdateOrderRequest request, string sellerId);
        public Task DeleteOrderAsync(int orderId, string sellerId);
        public Task<List<OrderResponse>> GetAllOrdersByShopAsync(int shopId, string sellerId);
        public Task<List<Order>> GetOrdersByShopOrThrowAsync(Shop shop);
        public Task<List<Order>> GetOrdersBuBuyerOrThrowAsync(ApplicationUser user);
    }
}
