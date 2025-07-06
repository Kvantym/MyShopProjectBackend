using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyShopProjectBackend.Db;
using MyShopProjectBackend.DTO;
using MyShopProjectBackend.Exceptions;
using MyShopProjectBackend.Models;
using MyShopProjectBackend.Servises.Interface;
using MyShopProjectBackend.ViewModels.Update;

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
        public Task CreateOrderAsync(string userId, List<OrderItemDto> orderItems)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteOrderAsync(int orderId, string sellerId)
        {
            var order = await _context.orders.Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product).ThenInclude(p => p.Shop)
                .FirstOrDefaultAsync(o => o.Id == orderId);
            if (order == null)
            {
                throw new NotFoundException("Замовлення не знайдено");
            }

            bool isSellerOwner = order.OrderItems.Any(oi => oi.Product.Shop.OwnerId == sellerId);
            if (!isSellerOwner)
            {
                throw new AuthorizationException("Ви не маєте доступу до цього замовлення");
            }

            var orderItems = await _context.orderItems.Where(oi => oi.OrderId == orderId).ToListAsync();
            _context.orderItems.RemoveRange(orderItems);
            _context.orders.Remove(order);
            await _context.SaveChangesAsync();
        }
        public async Task<List<OrderDto>> GetAllOrdersAsync(int shopId, string sellerId)
        {
            var user = await _userManager.FindByIdAsync(sellerId.ToString());
            if (user == null)
            {
                throw new NotFoundException("Користувача не знайдено");
            }

            var roles = await _userManager.GetRolesAsync(user);
            if (!roles.Contains("Seller"))
            {
                throw new AuthorizationException("Можна тільки продавцю");
            }

            var shop = await _context.shops.FindAsync(shopId);
            if (shop == null || shop.OwnerId != sellerId)
            {
                throw new NotFoundException("Магазин не знайдено або ви не є його власником");
            }

            var orders = await _context.orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.ShopOrders)
                .Where(o => o.ShopOrders.Any(so => so.ShopId == shopId))
                .ToListAsync();

            if (!orders.Any())
            {
                throw new NotFoundException("Замовлення не знайдено для цього магазину");
            }

            var orderDtos = GetOrderDtos(orders);
            return orderDtos;
        }

        public async Task<OrderDto?> GetOrderByIdAsync(int orderId, string sellerId)
        {
            var order = await _context.orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                        .ThenInclude(p => p.Shop)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                throw new NotFoundException("Замовлення не знайдено");
            }

            var seller = await _userManager.FindByIdAsync(sellerId.ToString());
            if (seller == null)
            {
                throw new NotFoundException("Користувача не знайдено");
            }

            var roles = await _userManager.GetRolesAsync(seller);
            if (!roles.Contains("Seller"))
            {
                throw new AuthorizationException("Можна тільки продавцю");
            }

            bool isSellerOwner = order.OrderItems.Any(oi => oi.Product.Shop.OwnerId == sellerId);
            if (!isSellerOwner)
            {
                throw new AuthorizationException("Ви не маєте доступу до цього замовлення");
            }

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

            return orderDto;
        }

        public async Task<List<OrderDto>> GetOrdersForUserAsync(string buyerName, string sellerId)
        {
            var buyer = await _userManager.FindByNameAsync(buyerName);
            if (buyer == null)
            {
                throw new NotFoundException("Покупця не знайдено");
            }

            var seller = await _userManager.FindByIdAsync(sellerId);
            if (seller == null)
            {
                throw new NotFoundException("Продавця не знайдено");
            }

            var roles = await _userManager.GetRolesAsync(seller);
            if (!roles.Contains("Seller"))
            {
                throw new AuthorizationException("Можна тільки продавцю");
            }

            var orders = await _context.orders
                .Where(o => o.BuyerId == buyer.Id)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                        .ThenInclude(p => p.Shop)
                // Фільтрація: замовлення мають містити товари з магазинів продавця
                .Where(o => o.OrderItems.Any(oi => oi.Product.Shop.OwnerId == sellerId))
                .ToListAsync();

            if (orders == null || !orders.Any())
            {
                throw new NotFoundException("Замовлення не знайдено для цього користувача");
            }
            var orderDtos = GetOrderDtos(orders);
            return orderDtos;
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
        public async Task UpdateOrderStatusAsync(UpdateOrderModel model)
        {
            var user = await _userManager.FindByIdAsync(model.SellerId.ToString());
            if (user == null)
            {
                throw new NotFoundException("Користувача не знайдено");
            }

            var isSeller = await _userManager.IsInRoleAsync(user, "Seller");
            if (!isSeller)
            {
                throw new AuthorizationException("Доступ дозволено лише продавцям");
            }

            var order = await _context.orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == model.OrderId);

            if (order == null)
            {
                throw new NotFoundException("Замовлення не знайдено");
            }

            bool isSellerOwner = order.OrderItems.Any(oi => oi.Product.Shop.OwnerId == model.SellerId);
            if (!isSellerOwner)
            {
                throw new AuthorizationException("Ви не маєте доступу до цього замовлення");
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
                throw new BadRequestException("Невірний статус замовлення");
            }

            if (order.Status == ShopOrderStatus.Completed.ToString() ||
                order.Status == ShopOrderStatus.Cancelled.ToString())
            {
                throw new BadRequestException("Замовлення вже завершено або скасовано");
            }
            order.Status = model.Status.ToString();

            await _context.SaveChangesAsync();
        }

    }
}
