using MyShopProjectBackend.DTO;
using MyShopProjectBackend.Models.Cart;

namespace MyShopProjectBackend.Servises.Interface
{
    public interface ICartServises
    {
        public  Task<CartDto?> GetCartAsync(string userId);
        public Task AddToCartAsync(AddToCartModel model, string userId);
        public Task UpdateCartAsync(UpdateCartModel model, string userId);
        public Task RemoveFromCartAsync(int productId, string userId);
        public Task ClearCartAsync(string userId);
        public Task CheckoutAsync(string userId);

    }
}
