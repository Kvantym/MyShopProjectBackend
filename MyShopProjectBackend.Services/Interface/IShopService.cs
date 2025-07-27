using MyShopProjectBackend.Domain.Entities;
using MyShopProjectBackend.Domain.Request.Shop;
using MyShopProjectBackend.Domain.Responses;

namespace MyShopProjectBackend.Services.Interface
{
    public interface IShopService
    {
        public Task CreateShopAsync(CreateShopRequest request, string ownerId);
        public Task UpdateShopAsync(UpdateShopRequest request, string ownerId);
        public Task DeleteShopAsync(int shopId, string ownerId);
        public Task<List<ShopResponse>> GetAllShopsAsync(string OwnerId);
        public Task<ShopResponse?> GetShopByIdAsync(int shopId);
        public Task<Shop> EnsureSellerOwnsShopAsync(ApplicationUser seller, int shopId);
        public Task<Shop> EnsureSellerOwnsShopForProductAsync(ApplicationUser seller, Product product);
        public Task<Shop> GetShopOrThrowAsync(int shopId);
        public Task<Shop> GetShopByNameOrThrowAsync(string shopName);
    }
}
