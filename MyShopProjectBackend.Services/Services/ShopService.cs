using MyShopProjectBackend.Domain.Entities;
using MyShopProjectBackend.Domain.Request.Shop;
using MyShopProjectBackend.Domain.Responses;
using MyShopProjectBackend.Repositories.Interfaces;
using MyShopProjectBackend.Services.Exceptions;
using MyShopProjectBackend.Services.Interface;

namespace MyShopProjectBackend.Services.Services
{
    public class ShopService : IShopService
    {
        private readonly IShopRepository _shopRepository;
        private readonly IUserService _userService;

        public ShopService(IShopRepository shopRepository, IUserService userService)
        {
            _shopRepository = shopRepository;
            _userService = userService;
        }
        public async Task CreateShopAsync(CreateShopRequest request, string ownerId)
        {
            var user = await _userService.GetUserOrThrowAsyncId(ownerId);

            var shop = await GetShopByNameOrThrowAsync(request.Name);

            var newShop = new Shop { Name = request.Name, Description = request.Description, OwnerId = user.Id };

            await _shopRepository.AddAsync(newShop);
        }

        public async Task DeleteShopAsync(int shopId, string ownerId)
        {
            var user = await _userService.GetUserOrThrowAsyncId(ownerId);

            var shop = await EnsureSellerOwnsShopAsync(user, shopId);

            await _shopRepository.DeleteAsync(shop);
        }

        public async Task<Shop> EnsureSellerOwnsShopAsync(ApplicationUser seller, int shopId)
        {
            var shop = await GetShopOrThrowAsync(shopId);

            if (shop.OwnerId != seller.Id)
            {
                throw new UnauthorizedAccessException("You do not own this shop");
            }
            return shop;
        }

        public Task<Shop> EnsureSellerOwnsShopForProductAsync(ApplicationUser seller, Product product)
        {
            throw new NotImplementedException();
        }

        public async Task<List<ShopResponse>> GetAllShopsAsync(string ownerId)
        {
            var user = await _userService.GetUserOrThrowAsyncId(ownerId);
            var shops = await _shopRepository.GetAllByOwnerAsync(user);

            if (shops == null || shops.Count == 0)
            {
                throw new NotFoundException("No shops found for this user");
            }

            var shopResponses = shops.Select(s => new ShopResponse
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
                OwnerId = s.OwnerId
            }).ToList();

            return shopResponses;
        }

        public async Task<ShopResponse?> GetShopByIdAsync(int shopId)
        {
            var shop = await GetShopOrThrowAsync(shopId);

            var shopResponse = new ShopResponse
            {
                Id = shop.Id,
                Name = shop.Name,
                Description = shop.Description,
                OwnerId = shop.OwnerId
            };
            return shopResponse;
        }

        public async Task<Shop> GetShopOrThrowAsync(int shopId)
        {
            var shop = await _shopRepository.GetByIdAsync(shopId);
            if (shop == null)
            {
                throw new NotFoundException("Shop not found");
            }
            return shop;
        }
        public async Task<Shop> GetShopByNameOrThrowAsync(string shopName)
        {
            var shop = await _shopRepository.GetByNameAsync(shopName);
            if (shop != null)

            {
                throw new BadRequestException("You have shop with this name");
            }
            return shop;
        }

        public async Task UpdateShopAsync(UpdateShopRequest request, string ownerId)
        {
            var user = await _userService.GetUserOrThrowAsyncId(ownerId);

            var shop = await EnsureSellerOwnsShopAsync(user, request.ShopId);

            shop.Name = request.Name;
            shop.Description = request.Description;

            await _shopRepository.UpdateAsync(shop);
        }
    }
}
