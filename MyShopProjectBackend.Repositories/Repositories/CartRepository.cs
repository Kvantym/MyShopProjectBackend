using Microsoft.EntityFrameworkCore;
using MyShopProjectBackend.Domain.Entities;
using MyShopProjectBackend.Infrastructure;
using MyShopProjectBackend.Repositories.Interfaces;

namespace MyShopProjectBackend.Repositories.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly AppDbConect _context;

        public CartRepository(AppDbConect context)
        {
            _context = context;
        }
        public async Task AddCartItemAsync(CartItem item)
        {
            _context.CartItems.Add(item);
            await _context.SaveChangesAsync();
        }

        public async Task ClearCartAsync(Cart cart)
        {
            _context.CartItems.RemoveRange(cart.Items);
            cart.Items.Clear();
            await _context.SaveChangesAsync();
        }

        public async Task<Cart?> GetCartByUserAsync(ApplicationUser user)
        {
            return await _context.Carts.Include(c => c.Items).ThenInclude(ci => ci.Product).FirstOrDefaultAsync(c => c.UserId == user.Id);
        }

        public async Task RemoveCartItemAsync(CartItem product)
        {
            _context.CartItems.Remove(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCartItemAsync(CartItem item)
        {
            _context.CartItems.Update(item);
            await _context.SaveChangesAsync();
        }

        public async Task<CartItem> GetCartItemByProductAndUserAsync(Product product, ApplicationUser user)
        {
            return await _context.CartItems
           .Include(ci => ci.Cart)
           .FirstOrDefaultAsync(ci => ci.ProductId == product.Id && ci.Cart.UserId == user.Id);
        }
    }
}
