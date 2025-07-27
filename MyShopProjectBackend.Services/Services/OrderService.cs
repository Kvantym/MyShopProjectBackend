using MyShopProjectBackend.Domain.Entities;
using MyShopProjectBackend.Domain.Request.Order;
using MyShopProjectBackend.Domain.Responses;
using MyShopProjectBackend.Repositories.Interfaces;
using MyShopProjectBackend.Services.Exceptions;
using MyShopProjectBackend.Services.Interface;

namespace MyShopProjectBackend.Services.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUserService _userService;
        private readonly IShopService _shopService;

        public OrderService( IOrderRepository orderRepository, IUserService userService, IShopService shopService)
        {
            _orderRepository = orderRepository;
            _userService = userService;
            _shopService = shopService;
        }

        public async Task DeleteOrderAsync(int orderId, string sellerId)
        {
            var user = await _userService.GetUserOrThrowAsyncId(sellerId);

            var order = await GetOrderByIdThrowAsync(orderId);

            await _orderRepository.DeleteOrderAsync(order);
        }

        public async Task<List<OrderResponse>> GetAllOrdersByShopAsync(int shopId, string sellerId)
        {
            var shop = await _shopService.GetShopOrThrowAsync(shopId);

            var orders = await GetOrdersByShopOrThrowAsync(shop);

            var orderResponse = orders.Select(order => new OrderResponse
            {
                OrderId = order.Id,
                Status = order.Status,
                Items = order.OrderItems.Select(item => new OrderItemResponse
                {
                    ProductName = item.Product.Name,
                    Quantity = item.Quantity,
                    TotalPrice = item.Quantity * item.UnitPrice,
                }).ToList()
            }).ToList();
            return orderResponse;
        }

        public async Task<OrderResponse?> GetOrderByIdAsync(int orderId, string sellerId)
        {
            var user = await _userService.GetUserOrThrowAsyncId(sellerId);

            var order = await GetOrderByIdThrowAsync(orderId);

            return new OrderResponse
            {
                OrderId = order.Id,
                Status = order.Status,
                Items = order.OrderItems.Select(item => new OrderItemResponse
                {
                    ProductName = item.Product.Name,
                    Quantity = item.Quantity,
                    TotalPrice = item.Quantity * item.UnitPrice,
                }).ToList()
            };
        }

        public async Task<List<OrderResponse>> GetOrdersForUserAsync(string buyerName)
        {
            var user = await _userService.GetUserOrThrowAsyncName(buyerName);

            var orders = await GetOrdersBuBuyerOrThrowAsync(user);

            var orderResponse = orders.Select(order => new OrderResponse
            {
                OrderId = order.Id,
                Status = order.Status,
                Items = order.OrderItems.Select(item => new OrderItemResponse
                {
                    ProductName = item.Product.Name,
                    Quantity = item.Quantity,
                    TotalPrice = item.Quantity * item.UnitPrice,
                }).ToList()
            }).ToList();
            return orderResponse;
        }

        public async Task UpdateOrderStatusAsync(UpdateOrderRequest request, string sellerId)
        {
            var user = await _userService.GetUserOrThrowAsyncId(sellerId);

            var order = await GetOrderByIdThrowAsync(request.OrderId);

            order.Status = request.Status;

            await _orderRepository.UpdateOrderAsync(order);
        }
        public async Task<Order> GetOrderByIdThrowAsync(int orderId)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);
            if (order == null)
            {
                throw new KeyNotFoundException($"Order with ID {orderId} not found.");
            }
            return order;
        }
        public async Task<List<Order>> GetOrdersByShopOrThrowAsync(Shop shop)
        {
            var orders = await _orderRepository.GetOrdersByShopIdAsync(shop);
            if (orders == null || orders.Count == 0)
            {
                throw new NotFoundException("No orders found for this shop.");
            }
            return orders;
        }
        public async Task<List<Order>> GetOrdersBuBuyerOrThrowAsync(ApplicationUser user)
        {
            var orders = await _orderRepository.GetOrdersByBuyerAsync(user);
            if (orders == null || orders.Count == 0)
            {
                throw new NotFoundException("No orders found for this shop.");
            }
            return orders;
        }
    }
}
