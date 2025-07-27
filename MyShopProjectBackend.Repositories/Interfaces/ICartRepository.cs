using MyShopProjectBackend.Domain.Entities;
using MyShopProjectBackend.Domain.Responses;

namespace MyShopProjectBackend.Repositories.Interfaces
{
    public interface ICartRepository
    {
        Task<Cart?> GetCartByUserAsync(ApplicationUser user);
        Task AddCartItemAsync(CartItem item);
        Task UpdateCartItemAsync(CartItem item);
        Task RemoveCartItemAsync(Cart cart, CartItem cartItem);
        Task ClearCartAsync(Cart cart);
        Task<CartItem> GetCartItemByProductAndUserAsync(Product product, ApplicationUser user);
        Task<CartItem> GetCartItemAsync(Cart cart, int productId);
        Task<List<CartItem>> GetCartItemsAsync(Cart cart);
    }
}
