
using MyShopProjectBackend.Domain.Entities;

namespace MyShopProjectBackend.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order?> GetOrderByIdAsync(int orderId);
        Task<List<Order>> GetOrdersByBuyerAsync(ApplicationUser buyer);
        Task<List<Order>> GetOrdersByBuyerAndSellerAsync(ApplicationUser buyer, ApplicationUser seller);
        Task<List<Order>> GetOrdersByShopIdAsync(Shop shop);
        Task<Order?> GetOrderWithDetailsAsync(int orderId);
        Task<List<Order>> GetOrdersByShopAndSellerAsync(Shop shop, ApplicationUser seller);
        Task CreateOrderAsync(Order order);
        Task DeleteOrderAsync(Order order);
        Task UpdateOrderAsync(Order order);
    }
}
