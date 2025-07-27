using MyShopProjectBackend.Domain.Entities;
using MyShopProjectBackend.Domain.Request.Cart;
using MyShopProjectBackend.Domain.Responses;

namespace MyShopProjectBackend.Services.Interface
{
    public interface ICartService
    {
        public Task<CartResponse> GetCartAsync(string userId);
        public Task AddToCartAsync(AddToCartRequest request, string userId);
        public Task UpdateCartAsync(UpdateCartRequest request, string userId);
        public Task RemoveFromCartAsync(int productId, string userId);
        public Task ClearCartAsync(string userId);
        public Task CheckoutAsync(string userId);
        public Task<Cart> GetCartOrThrowAsync(ApplicationUser user);//перевірка кошика на наявність + виклик Exception, якщо кошика немає
    }
}
 