using MyShopProjectBackend.DTO;
using MyShopProjectBackend.ViewModels.Create;
using MyShopProjectBackend.ViewModels.Delete;
using MyShopProjectBackend.ViewModels.Update;

namespace MyShopProjectBackend.Servises.Interface
{
    public interface IShopServise
    {
        public Task CreateShopAsync(CreateShopModel model);
        public Task UpdateShopAsync(UpdateShopModel model);
        public Task DeleteShopAsync(DeleteShopModel model);
        public Task< List<ShopDto>> GetAllShopsAsync(string OwnerId);
        public Task<ShopDto?> GetShopByIdAsync(int shopId);
    }
}
