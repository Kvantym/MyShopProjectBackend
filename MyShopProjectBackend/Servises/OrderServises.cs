using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyShopProjectBackend.Db;
using MyShopProjectBackend.DTO;
using MyShopProjectBackend.Entities;
using MyShopProjectBackend.Exceptions;
using MyShopProjectBackend.Models.Order;
using MyShopProjectBackend.Servises.Interface;
using NLog;
using ILogger = NLog.ILogger;

namespace MyShopProjectBackend.Servises
{
    public class OrderServises : IOrderServises
    {
        private readonly AppDbConection _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public OrderServises(AppDbConection context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public Task CreateOrderAsync(string userId, List<OrderItemDto> orderItems)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteOrderAsync(int orderId, string sellerId)
        {
            _logger.Info($"{nameof(DeleteOrderAsync)}: Виклик методу");
            
            var user = await _userManager.FindByIdAsync(sellerId);
            if (user == null)
            {
                _logger.Warn($"{nameof(DeleteOrderAsync)}: Користувача з ID: {sellerId} не знайдено - завершилось помилкою");
                throw new NotFoundException("Користувача не знайдено");
            }

            var order = await _context.orders.Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product).ThenInclude(p => p.Shop)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                _logger.Warn($"{nameof(DeleteOrderAsync)}: Замовлення з ID: {orderId} не знайдено - завершилось помилкою");
                throw new NotFoundException("Замовлення не знайдено");
            }
            _logger.Info($"{nameof(DeleteOrderAsync)}: Видалення замовлення з ID: {order.Id} для продавця з ID: {user.UserName}");
            bool isSellerOwner = order.OrderItems.Any(oi => oi.Product.Shop.OwnerId == sellerId);
            if (!isSellerOwner)
            {
                _logger.Warn($"{nameof(DeleteOrderAsync)}: Відмова в доступі: продавець з ID {user.UserName} не є власником замовлення {orderId} - завершилось помилкою");
                throw new AuthorizationException("Ви не маєте доступу до цього замовлення");
            }

            var orderItems = await _context.orderItems.Where(oi => oi.OrderId == orderId).ToListAsync();
            _context.orderItems.RemoveRange(orderItems);
            _context.orders.Remove(order);
            await _context.SaveChangesAsync();

            _logger.Info($"{nameof(DeleteOrderAsync)}: Замовлення з ID: {order.Id} успішно видалено");
        }

        public async Task<List<OrderDto>> GetAllOrdersAsync(int shopId, string sellerId)
        {
            _logger.Info($"{nameof(GetAllOrdersAsync)}: Виклик методу");
            var user = await _userManager.FindByIdAsync(sellerId);
            if (user == null)
            {
                _logger.Warn($"{nameof(GetAllOrdersAsync)}: Користувача з ID: {sellerId} не знайдено - завершилось помилкою");
                throw new NotFoundException("Користувача не знайдено");
            }

            var roles = await _userManager.GetRolesAsync(user);
            if (!roles.Contains("Seller"))
            {
                _logger.Warn($"{nameof(GetAllOrdersAsync)}: Користувач з ID: {user.UserName} не є продавцем - завершилось помилкою");
                throw new AuthorizationException("Можна тільки продавцю");
            }

            var shop = await _context.shops.FindAsync(shopId);
            if (shop == null || shop.OwnerId != sellerId)
            {
                _logger.Warn($"{nameof(GetAllOrdersAsync)}: Магазин не знайдено або продавець не є його власником - завершилось помилкою");
                throw new NotFoundException("Магазин не знайдено або ви не є його власником");
            }

            var orders = await _context.orders
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
                .Include(o => o.ShopOrders)
                .Where(o => o.ShopOrders.Any(so => so.ShopId == shopId))
                .ToListAsync();

            if (orders.Count == 0)
            {
                _logger.Warn($"{nameof(GetAllOrdersAsync)}: Замовлення не знайдено для магазину {shop.Name} - завершилось помилкою");
                throw new NotFoundException("Замовлення не знайдено для цього магазину");
            }
            _logger.Info($"{nameof(GetAllOrdersAsync)}: Успішно виконано");
            return GetOrderDtos(orders);
        }

        public async Task<OrderDto?> GetOrderByIdAsync(int orderId, string sellerId)
        {
            _logger.Info($"{nameof(GetOrderByIdAsync)}: Виклик методу");
            var order = await _context.orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                        .ThenInclude(p => p.Shop)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                _logger.Warn($"{nameof(GetOrderByIdAsync)} : Замовлення з ID: {orderId} не знайдено - завершилось помилкою");
                throw new NotFoundException("Замовлення не знайдено");
            }

            var seller = await _userManager.FindByIdAsync(sellerId);
            if (seller == null)
            {
                _logger.Warn($"{nameof(GetOrderByIdAsync)}: Користувача з ID: {sellerId} не знайдено - завершилось помилкою");
                throw new NotFoundException("Користувача не знайдено");
            }

            var roles = await _userManager.GetRolesAsync(seller);
            if (!roles.Contains("Seller"))
            {
                _logger.Warn($"{nameof(GetOrderByIdAsync)}: Користувач з ID: {seller.UserName} не є продавцем - завершилось помилкою");
                throw new AuthorizationException("Можна тільки продавцю");
            }

            bool isSellerOwner = order.OrderItems.Any(oi => oi.Product.Shop.OwnerId == sellerId);
            if (!isSellerOwner)
            {
                _logger.Warn($"{nameof(GetOrderByIdAsync)}: Користувач : {seller.UserName} не має доступу до замовлення - завершилось помилкою");
                throw new AuthorizationException("Ви не маєте доступу до цього замовлення");
            }

var orderDtos = new OrderDto
            {
                OrderId = order.Id,
                Status = order.Status,
                Items = order.OrderItems.Select(oi => new OrderItemDto
                {
                    ProductId = oi.ProductId,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice,
                    ProductName = oi.Product.Name
                }).ToList()
            };
            _logger.Info($"{nameof(GetOrderByIdAsync)}: Успішно виконано");
            return orderDtos;
        }

        public async Task<List<OrderDto>> GetOrdersForUserAsync(string buyerName, string sellerId)
        {
            _logger.Info($"{nameof(GetOrdersForUserAsync)}: Виклик методу");
            var buyer = await _userManager.FindByNameAsync(buyerName);
            if (buyer == null)
            {
                _logger.Warn($"{nameof(GetOrdersForUserAsync)}: Покупця з ім'ям {buyerName} не знайдено - завершилось помилкою");
                throw new NotFoundException("Покупця не знайдено");
            }

            var seller = await _userManager.FindByIdAsync(sellerId);
            if (seller == null)
            {
                _logger.Warn($"{nameof(GetOrdersForUserAsync)}: Продавця з ID {sellerId} не знайдено - завершилось помилкою");
                throw new NotFoundException("Продавця не знайдено");
            }

            var roles = await _userManager.GetRolesAsync(seller);
            if (!roles.Contains("Seller"))
            {
                _logger.Warn($"{nameof(GetOrdersForUserAsync)}: Користувач {seller.UserName} не має ролі 'Seller' - завершилось помилкою");
                throw new AuthorizationException("Можна тільки продавцю");
            }

            var orders = await _context.orders
                .Where(o => o.BuyerId == buyer.Id)
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Product).ThenInclude(p => p.Shop)
                .Where(o => o.OrderItems.Any(oi => oi.Product.Shop.OwnerId == sellerId))
                .ToListAsync();

            if (orders.Count == 0)
            {
                _logger.Warn($"{nameof(GetOrdersForUserAsync)}: Замовлення для покупця {buyer.UserName} у продавця {seller.UserName} не знайдено - завершилось помилкою");
                throw new NotFoundException("Замовлення не знайдено для цього користувача");
            }
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

        public async Task UpdateOrderStatusAsync(UpdateOrderModel model, string sellerId)
        {
            _logger.Info($"{nameof(UpdateOrderStatusAsync)}: Виклик методу");

            var user = await _userManager.FindByIdAsync(sellerId);
            if (user == null)
            {
                _logger.Warn($"{nameof(UpdateOrderStatusAsync)}: Користувача з ID {sellerId} не знайдено - завершилось помилкою");
                throw new NotFoundException("Користувача не знайдено");
            }

            var isSeller = await _userManager.IsInRoleAsync(user, "Seller");
            if (!isSeller)
            {
                _logger.Warn($"{nameof(UpdateOrderStatusAsync)}: Користувач {user.UserName} не має прав продавця - завершилось помилкою");
                throw new AuthorizationException("Доступ дозволено лише продавцям");
            }

            var order = await _context.orders.Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == model.OrderId);

            if (order == null)
            {
                _logger.Warn($"{nameof(UpdateOrderStatusAsync)}: Замовлення з ID {model.OrderId} не знайдено - завершилось помилкою");
                throw new NotFoundException("Замовлення не знайдено");
            }

            bool isSellerOwner = order.OrderItems.Any(oi => oi.Product.Shop.OwnerId == sellerId);
            if (!isSellerOwner)
            {
                _logger.Warn($"{nameof(UpdateOrderStatusAsync)}: Користувач {user.UserName} не має доступу до замовлення - завершилось помилкою");
                throw new AuthorizationException("Ви не маєте доступу до цього замовлення");
            }

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
    }
}
