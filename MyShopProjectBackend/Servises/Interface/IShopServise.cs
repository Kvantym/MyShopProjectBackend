using MyShopProjectBackend.DTO;
using MyShopProjectBackend.Models.Shop;

namespace MyShopProjectBackend.Servises.Interface
{
    public interface IShopServise
    {
        public Task CreateShopAsync(CreateShopModel model, string ownerId);
        public Task UpdateShopAsync(UpdateShopModel model, string ownerId);
        public Task DeleteShopAsync(int shopId, string ownerId);
        public Task< List<ShopDto>> GetAllShopsAsync(string OwnerId);
        public Task<ShopDto?> GetShopByIdAsync(int shopId);
    }
}
