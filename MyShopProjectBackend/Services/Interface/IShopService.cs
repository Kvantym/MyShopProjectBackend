using MyShopProjectBackend.DTO;
using MyShopProjectBackend.Entities;
using MyShopProjectBackend.Models.Shop;

namespace MyShopProjectBackend.Servises.Interface
{
    public interface IShopService
    {
        public Task CreateShopAsync(CreateShopModel model, string ownerId);
        public Task UpdateShopAsync(UpdateShopModel model, string ownerId);
        public Task DeleteShopAsync(int shopId, string ownerId);
        public Task< List<ShopDto>> GetAllShopsAsync(string OwnerId);
        public Task<ShopDto?> GetShopByIdAsync(int shopId);
        public Task<Shop> EnsureSellerOwnsShopAsync(ApplicationUser seller, int shopId);
        public Task<Shop> EnsureSellerOwnsShopForProductAsync(ApplicationUser seller, Product product);
        public Task<Shop> GetShopOrThrowAsync (int shopId);

    }
}
