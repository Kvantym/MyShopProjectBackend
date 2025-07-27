using Microsoft.EntityFrameworkCore;
using MyShopProjectBackend.Domain.Entities;
using MyShopProjectBackend.Domain.Responses;
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

        public async Task RemoveCartItemAsync(Cart cart, CartItem cartItem)
        {
            cart.Items.Remove(cartItem);
            _context.CartItems.Remove(cartItem);
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
       public  async Task<CartItem> GetCartItemAsync(Cart cart, int productId)
        {
           var cartItem = await _context.CartItems.Include(ci=> ci.Product).FirstOrDefaultAsync(ci => ci.CartId == cart.Id && ci.ProductId == productId);
            return cartItem;
        }

      public async Task<List<CartItem>> GetCartItemsAsync(Cart cart)
        {
          var cartItems =await _context.CartItems
                .Include(ci => ci.Product)
                .Where(ci => ci.CartId == cart.Id)
                .ToListAsync();
            return cartItems;
        }

    }
}
