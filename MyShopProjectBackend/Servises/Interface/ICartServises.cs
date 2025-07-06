using MyShopProjectBackend.DTO;
using MyShopProjectBackend.ViewModels.Add;
using MyShopProjectBackend.ViewModels.Remove;
using MyShopProjectBackend.ViewModels.Update;

namespace MyShopProjectBackend.Servises.Interface
{
    public interface ICartServises
    {
        public  Task<CartDto?> GetCartAsync(string userId);
        public Task AddToCartAsync(AddToCartModel model);
        public Task UpdateCartAsync(UpdateCartModel model);
        public Task RemoveFromCartAsync(RemoveCartModel model);
        public Task ClearCartAsync(string userId);
        public Task CheckoutAsync(string userId);

    }
}
