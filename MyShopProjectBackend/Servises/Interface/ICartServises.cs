using MyShopProjectBackend.DTO;
using MyShopProjectBackend.ViewModels;

namespace MyShopProjectBackend.Servises.Interface
{
    public interface ICartServises
    {
        public  Task<(bool Success, string? ErrorMessage, CartDto? CartDto)> GetCartAsync(int userId);
        public Task<(bool Success, string? ErrorMessage)> AddToCartAsync(AddToCartModel model);
        public Task<(bool Success, string? ErrorMessage)> UpdateCartAsync(UpdateCartModel model);
        public Task<(bool Success, string? ErrorMessage)> RemoveFromCartAsync(RemoveCartModel model);
        public Task<(bool Success, string? ErrorMessage)> ClearCartAsync(int userId);
        public Task<(bool Success, string? ErrorMessage)> CheckoutAsync(int userId);

    }
}
