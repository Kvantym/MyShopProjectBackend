using MyShopProjectBackend.Domain.Entities;

namespace MyShopProjectBackend.Repositories.Interfaces
{
    public interface ICartRepository
    {
        Task<Cart?> GetCartByUserAsync(ApplicationUser user);
        Task AddCartItemAsync(CartItem item);
        Task UpdateCartItemAsync(CartItem item);
        Task RemoveCartItemAsync(CartItem product);
        Task ClearCartAsync(Cart cart);
        Task<CartItem> GetCartItemByProductAndUserAsync(Product product, ApplicationUser user);
    }
}
