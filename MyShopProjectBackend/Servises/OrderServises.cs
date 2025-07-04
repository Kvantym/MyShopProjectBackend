using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MyShopProjectBackend.Db;
using MyShopProjectBackend.DTO;
using MyShopProjectBackend.Models;
using MyShopProjectBackend.Servises.Interface;
using MyShopProjectBackend.ViewModels;

namespace MyShopProjectBackend.Servises
{
    public class OrderServises : IOrderServises
    {
        private readonly AppDbConection _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrderServises(AppDbConection context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public Task<(bool Success, string? ErrorMessage)> CreateOrderAsync(int userId, List<OrderItemDto> orderItems)
        {
            throw new NotImplementedException();
        }

        public async Task<(bool Success, string? ErrorMessage)> DeleteOrderAsync(int orderId, int sellerId)
        {
            var order = await _context.orders.Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product).ThenInclude(p => p.Shop)
                .FirstOrDefaultAsync(o => o.Id == orderId);
            if (order == null)
            {
                return (false,"Замовлення не знайдено");
            }

            bool isSellerOwner = order.OrderItems.Any(oi => oi.Product.Shop.OwnerId == sellerId);
            if (!isSellerOwner)
            {
                return (false, "Ви не маєте доступу до цього замовлення");
            }

            var orderItems = await _context.orderItems.Where(oi => oi.OrderId == orderId).ToListAsync();
            _context.orderItems.RemoveRange(orderItems);
            _context.orders.Remove(order);
            await _context.SaveChangesAsync();

            return (true, null);
        }
        public async Task<(bool Success, string? ErrorMessage, List<OrderDto> Orders)> GetAllOrdersAsync(int shopId, int sellerId)
        {
            var user = await _userManager.FindByIdAsync(sellerId.ToString());
            if (user == null)
            {
                return (false, "Користувача не знайдено", new List<OrderDto>());
            }

            var roles = await _userManager.GetRolesAsync(user);
            if (!roles.Contains("Seller"))
            {
                return (false, "Користувач не є продавцем", new List<OrderDto>());
            }

            var shop = await _context.shops.FindAsync(shopId);
            if (shop == null || shop.OwnerId != sellerId)
            {
                return (false, "Магазин не знайдено або ви не є його власником", new List<OrderDto>());
            }

            var orders = await _context.orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.ShopOrders)
                .Where(o => o.ShopOrders.Any(so => so.ShopId == shopId))
                .ToListAsync();

            if (!orders.Any())
            {
                return (false, "Замовлення не знайдено", new List<OrderDto>());
            }

            var orderDtos = GetOrderDtos(orders);

            return (true, null, orderDtos);
        }

        public async Task<(bool Success, string? ErrorMessage, OrderDto? Order)> GetOrderByIdAsync(int orderId, int sellerId)
        {
            var order = await _context.orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                        .ThenInclude(p => p.Shop)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                return (false, "Замовлення не знайдено", null);

            var seller = await _userManager.FindByIdAsync(sellerId.ToString());
            if (seller == null)
                return (false, "Користувача не знайдено", null);

            // Отримуємо ролі користувача
            var roles = await _userManager.GetRolesAsync(seller);
            if (!roles.Contains("Seller"))
                return (false, "Можна тільки продавцю", null);

            bool isSellerOwner = order.OrderItems.Any(oi => oi.Product.Shop.OwnerId == sellerId);
            if (!isSellerOwner)
                return (false, "Ви не маєте доступу до цього замовлення", null);

            var orderDto = new OrderDto
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

            return (true, null, orderDto);
        }

        public async Task<(bool Success, string? ErrorMessage, List<OrderDto> Orders)> GetOrdersForUserAsync(int buyerId, int sellerId)
        {
            var buyer = await _userManager.FindByIdAsync(buyerId.ToString());
            if (buyer == null)
            {
                return (false, "Покупця не знайдено", new List<OrderDto>());
            }

            var seller = await _userManager.FindByIdAsync(sellerId.ToString());
            if (seller == null)
            {
                return (false, "Продавця не знайдено", new List<OrderDto>());
            }

            // Отримуємо ролі продавця
            var roles = await _userManager.GetRolesAsync(seller);
            if (!roles.Contains("Seller"))
            {
                return (false, "Можна тільки продавцю", new List<OrderDto>());
            }

            var orders = await _context.orders
                .Where(o => o.BuyerId == buyerId)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                        .ThenInclude(p => p.Shop)
                // Фільтрація: замовлення мають містити товари з магазинів продавця
                .Where(o => o.OrderItems.Any(oi => oi.Product.Shop.OwnerId == sellerId))
                .ToListAsync();

            if (orders == null || !orders.Any())
            {
                return (false, "Замовлення не знайдено для цього користувача", new List<OrderDto>());
            }

            var orderDtos = GetOrderDtos(orders);

            return (true, null, orderDtos);
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
        public async Task<(bool Success, string? ErrorMessage)> UpdateOrderStatusAsync(UpdateOrderModel model)
        {
            var user = await _userManager.FindByIdAsync(model.SellerId.ToString());
            if (user == null)
            {
                return (false, "Користувача не знайдено");
            }

            // Перевірка ролі через IsInRoleAsync
            var isSeller = await _userManager.IsInRoleAsync(user, "Seller");
            if (!isSeller)
            {
                return (false, "Доступ дозволено лише продавцям");
            }

            var order = await _context.orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == model.OrderId);

            if (order == null)
            {
                return (false, "Замовлення не знайдено");
            }

            // Перевірка, що продавець є власником хоча б одного товару в замовленні
            bool isSellerOwner = order.OrderItems.Any(oi => oi.Product.Shop.OwnerId == model.SellerId);
            if (!isSellerOwner)
            {
                return (false, "Ви не маєте доступу до цього замовлення");
            }

            var validStatuses = new[]
            {
        ShopOrderStatus.Pending,
        ShopOrderStatus.Completed,
        ShopOrderStatus.Cancelled,
        ShopOrderStatus.InProgress,
        ShopOrderStatus.Refunded,
        ShopOrderStatus.Shipped,
        ShopOrderStatus.Delivered,
        ShopOrderStatus.Confirmed
    };

            if (!validStatuses.Contains(model.Status))
            {
                return (false, "Невірний статус");
            }

            // Перевірка, чи замовлення вже завершене або скасоване
            if (order.Status == ShopOrderStatus.Completed.ToString() ||
                order.Status == ShopOrderStatus.Cancelled.ToString())
            {
                return (false, "Замовлення вже завершено або скасовано");
            }

            // Оновлення статусу (зберігаємо рядок від enum)
            order.Status = model.Status.ToString();

            await _context.SaveChangesAsync();

            return (true, null);
        }

    }
}
