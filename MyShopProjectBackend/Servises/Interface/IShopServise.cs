using MyShopProjectBackend.DTO;
using MyShopProjectBackend.ViewModels;

namespace MyShopProjectBackend.Servises.Interface
{
    public interface IShopServise
    {
        public Task<(bool Success, string? ErrorMessage)> CreateShopAsync(CreateShopModel model);
        public Task<(bool Success, string? ErrorMessage)> UpdateShopAsync(UpdateShopModel model);
        public Task<(bool Success, string? ErrorMessage)> DeleteShopAsync(DeleteShopModel model);
        public Task<(bool Success, string? ErrorMessage, List<ShopDto> Shops)> GetAllShopsAsync(int OwnerId);
        public Task<(bool Success, string? ErrorMessage, ShopDto? Shop)> GetShopByIdAsync(int shopId);
    }
}
