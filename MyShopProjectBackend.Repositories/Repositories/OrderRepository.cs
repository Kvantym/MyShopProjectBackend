using Microsoft.EntityFrameworkCore;
using MyShopProjectBackend.Domain.Entities;
using MyShopProjectBackend.Infrastructure;
using MyShopProjectBackend.Repositories.Interfaces;

namespace MyShopProjectBackend.Repositories.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbConect _context;
            public OrderRepository(AppDbConect context)
            {
                _context = context;
            }
    
            public async Task<Order?> GetOrderByIdAsync(int orderId)
            {
                return await _context.Orders.FindAsync(orderId);
            }
    
            public async Task<List<Order>> GetOrdersByBuyerAsync(ApplicationUser buyer)
            {
                return await _context.Orders.Where(o => o.Buyer.Id == buyer.Id).ToListAsync();
            }
    
            public async Task<List<Order>> GetOrdersByBuyerAndSellerAsync(ApplicationUser buyer, ApplicationUser seller)
            {
                return await _context.Orders
                    .Where(o => o.Buyer.Id == buyer.Id && o.Seller.Id == seller.Id)
                    .ToListAsync();
            }
    
            public async Task<List<Order>> GetOrdersByShopIdAsync(Shop shop)
            {
                return await _context.Orders.Where(o => o.Shop.Id == shop.Id).ToListAsync();
            }
    
            public async Task<Order?> GetOrderWithDetailsAsync(int orderId)
            {
                return await _context.Orders
                    .Include(o => o.OrderItems)
                    .FirstOrDefaultAsync(o => o.Id == orderId);
            }
    
            public async Task<List<Order>> GetOrdersByShopAndSellerAsync(Shop shop, ApplicationUser seller)
            {
                return await _context.Orders
                    .Where(o => o.Shop.Id == shop.Id && o.Seller.Id == seller.Id)
                    .ToListAsync();
            }
    
            public async Task CreateOrderAsync(Order order)
            {
                await _context.Orders.AddAsync(order);
                await _context.SaveChangesAsync();
            }
    
            public async Task DeleteOrderAsync(Order order)
            {
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
            }
    
            public async Task UpdateOrderAsync(Order order)
            {
                _context.Orders.Update(order);
                await _context.SaveChangesAsync();
        }
    }
}
