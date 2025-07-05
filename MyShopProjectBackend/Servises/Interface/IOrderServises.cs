using MyShopProjectBackend.DTO;
using MyShopProjectBackend.ViewModels;

namespace MyShopProjectBackend.Servises.Interface
{
    public interface IOrderServises
    {
        public Task<(bool Success, string? ErrorMessage, List<OrderDto> Orders)> GetOrdersForUserAsync(int buyerId, int sellerId);
        public Task<(bool Success, string? ErrorMessage, OrderDto? Order)> GetOrderByIdAsync(int orderId, int sellerId);
        public Task<(bool Success, string? ErrorMessage)> UpdateOrderStatusAsync(UpdateOrderModel model);
        public Task<(bool Success, string? ErrorMessage)> CreateOrderAsync(int userId, List<OrderItemDto> orderItems);
        public Task<(bool Success, string? ErrorMessage)> DeleteOrderAsync(int orderId, int sellerId);
        public Task<(bool Success, string? ErrorMessage, List<OrderDto> Orders)> GetAllOrdersAsync(int shopId, int sellerId);
    }
}
