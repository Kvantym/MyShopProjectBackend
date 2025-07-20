using Microsoft.EntityFrameworkCore;
using MyShopProjectBackend.Db;
using MyShopProjectBackend.DTO;
using MyShopProjectBackend.Entities;
using MyShopProjectBackend.Exceptions;
using MyShopProjectBackend.Models.Order;
using MyShopProjectBackend.Servises.Interface;
using NLog;
using ILogger = NLog.ILogger;

namespace MyShopProjectBackend.Services.Implementation
{
    //перероблено сервіс
    public class OrderService : IOrderService
    {
        private readonly AppDbConection _context;
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly IUserService _userService;
        private readonly IShopService _shopService;

        public OrderService(AppDbConection context, IUserService userService, IShopService shopService)
        {
            _context = context;
            _userService = userService;
            _shopService = shopService;
        }

        public Task CreateOrderAsync(string userId, List<OrderItemDto> orderItems)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteOrderAsync(int orderId, string sellerId)//готово
        {
            _logger.Info($"{nameof(DeleteOrderAsync)}: Виклик методу");
            
            var user = await _userService.GetUserOrThrowAsyncId(sellerId);

            var order = await GetOrderWithDetailsAsync(orderId);

            _logger.Info($"{nameof(DeleteOrderAsync)}: Видалення замовлення з ID: {order.Id} для продавця з ID: {user.UserName}");
            bool isSellerOwner = order.OrderItems.Any(oi => oi.Product.Shop.OwnerId == sellerId);
            if (!isSellerOwner)
            {
                _logger.Warn($"{nameof(DeleteOrderAsync)}: Відмова в доступі: продавець з ID {user.UserName} не є власником замовлення {orderId}");
                throw new AuthorizationException("Ви не маєте доступу до цього замовлення");
            }

            var orderItems = await _context.orderItems.Where(oi => oi.OrderId == orderId).ToListAsync();
            _context.orderItems.RemoveRange(orderItems);
            _context.orders.Remove(order);
            await _context.SaveChangesAsync();

            _logger.Info($"{nameof(DeleteOrderAsync)}: Замовлення з ID: {order.Id} успішно видалено");
        }

        public async Task<List<OrderDto>> GetAllOrdersAsync(int shopId, string sellerId)//готово
        {
            _logger.Info($"{nameof(GetAllOrdersAsync)}: Виклик методу");
            var seller = await _userService.GetUserOrThrowAsyncId(sellerId);

            var roles = await _userService.EnsureUserHasRoleOrThrowAsync(seller, "Seller");

            var shop = await _shopService.EnsureSellerOwnsShopAsync(seller, shopId);

            var orders = await GetOrdersByShopIdAsync(seller, shop);

            _logger.Info($"{nameof(GetAllOrdersAsync)}: Успішно виконано");
            return GetOrderDtos(orders);
        }

        public async Task<OrderDto?> GetOrderByIdAsync(int orderId, string sellerId)//готово
        {
            _logger.Info($"{nameof(GetOrderByIdAsync)}: Виклик методу");

            var order = await GetOrderWithDetailsAsync(orderId);

            var seller = await _userService.GetUserOrThrowAsyncId(sellerId);

            var roles = await _userService.EnsureUserHasRoleOrThrowAsync(seller, "Seller");

            _userService.EnsureSellerOwnsOrder(order, seller);

            _logger.Info($"{nameof(GetOrderByIdAsync)}: Успішно виконано");
            return GetOrderDtos(new List<Order> { order }).First();
        }

        public async Task<List<OrderDto>> GetOrdersForUserAsync(string buyerName, string sellerId)//готово
        {
            _logger.Info($"{nameof(GetOrdersForUserAsync)}: Виклик методу");

            var buyer = await _userService.GetUserOrThrowAsyncName(buyerName);

            var seller = await _userService.GetUserOrThrowAsyncId(sellerId);
            
            var roles = await _userService.EnsureUserHasRoleOrThrowAsync(seller, "Seller");

            var orders = await GetOrdersByBuyerAndSellerAsync(buyer, seller);

            _logger.Info($"{nameof(GetOrdersForUserAsync)}: Успішно виконано");
            return GetOrderDtos(orders);
        }

        private List<OrderDto> GetOrderDtos(List<Order> orders)
        {
            return orders.Select(o => new OrderDto
            {
                OrderId = o.Id,
                Status = o.Status,
                Items = o.OrderItems.Select(oi => new OrderItemDto
                {
                    ProductId = oi.ProductId,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice,
                    ProductName = oi.Product.Name
                }).ToList()
            }).ToList();
        }

        public async Task UpdateOrderStatusAsync(UpdateOrderModel model, string sellerId)//готово
        {
            _logger.Info($"{nameof(UpdateOrderStatusAsync)}: Виклик методу");

            var seller = await _userService.GetUserOrThrowAsyncId(sellerId);

            var isSeller = await _userService.EnsureUserHasRoleOrThrowAsync(seller, "Seller");

            var order = await GetOrderWithDetailsAsync(model.OrderId);

            _userService.EnsureSellerOwnsOrder(order, seller);

            var validStatuses = new[]
            {
                ShopOrderStatus.Pending,
                ShopOrderStatus.Completed,
                ShopOrderStatus.Cancelled,
                ShopOrderStatus.Refunded,
                ShopOrderStatus.InProgress
            };

            if (!validStatuses.Contains(model.Status))
            {
                _logger.Warn($"{nameof(UpdateOrderStatusAsync)}: Невідомий статус: {model.Status} - завершилось помилкою");
                throw new ArgumentException("Невідомий статус замовлення");
            }

            order.Status = model.Status;
            await _context.SaveChangesAsync();

            _logger.Info($"{nameof(UpdateOrderStatusAsync)}: Статус замовлення {order.Id} оновлено до {order.Status}, {nameof(UpdateOrderStatusAsync)}:  Успішно виконано");
        }
        public async Task<Order> GetOrderWithDetailsAsync(int orderId)
        {
            var order = await _context.orders
               .Include(o => o.OrderItems)
                   .ThenInclude(oi => oi.Product)
                       .ThenInclude(p => p.Shop)
               .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                _logger.Warn($"{nameof(GetOrderWithDetailsAsync)} : Замовлення з ID: {orderId} не знайдено - завершилось помилкою");
                throw new NotFoundException("Замовлення не знайдено");
            }
            return order;
        }
        public async Task<List<Order>> GetOrdersByBuyerAndSellerAsync(ApplicationUser buyer, ApplicationUser seller)
        {
            var orders = await _context.orders
               .Where(o => o.BuyerId == buyer.Id)
               .Include(o => o.OrderItems).ThenInclude(oi => oi.Product).ThenInclude(p => p.Shop)
               .Where(o => o.OrderItems.Any(oi => oi.Product.Shop.OwnerId == seller.Id))
               .ToListAsync();

            if (orders.Count == 0)
            {
                _logger.Warn($"{nameof(GetOrdersForUserAsync)}: Замовлення для покупця {buyer.UserName} у продавця {seller.UserName} не знайдено");
                throw new NotFoundException("Замовлення не знайдено для цього користувача");
            }
            return orders;
        }
        public async Task<List<Order>> GetOrdersByShopIdAsync(ApplicationUser seller, Shop shop)
        {
            var orders = await _context.orders
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
                .Include(o => o.ShopOrders)
                .Where(o => o.ShopOrders.Any(so => so.ShopId == shop.Id))
                .ToListAsync();

            if (orders.Count == 0)
            {
                _logger.Warn($"{nameof(GetAllOrdersAsync)}: Замовлення не знайдено для магазину {shop.Name}");
                throw new NotFoundException("Замовлення не знайдено для цього магазину");
            }
            return orders;
        }
    }
}
//270