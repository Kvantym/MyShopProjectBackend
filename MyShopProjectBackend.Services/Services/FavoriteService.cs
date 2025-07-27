using MyShopProjectBackend.Domain.Entities;
using MyShopProjectBackend.Domain.Request.Favorite;
using MyShopProjectBackend.Repositories.Interfaces;
using MyShopProjectBackend.Services.Interface;

namespace MyShopProjectBackend.Services.Services
{
    public class FavoriteService : IFavoriteService
    {
        private readonly IFavoriteRepository _favoriteRepository;
        private readonly IUserService _userService;
        private readonly IProductService _productService;
        public FavoriteService( IFavoriteRepository favoriteRepository, IUserService userService, IProductService productService)
        {
            _favoriteRepository = favoriteRepository;
            _userService = userService;
            _productService = productService;
        }
        public async Task AddToFavoritesAsync(AddFavoritRequest request, string userId)
        {
            var user = await _userService.GetUserOrThrowAsyncId(userId);

            var product = await _productService.GetProductOrThrowIdAsync(request.ProductId);

            var favoriteProduct = new FavouriteProduct
            {
                ProductId = request.ProductId,
                UserId = userId,
                Product = product
            };
            await _favoriteRepository.AddToFavoritesAsync(favoriteProduct);
        }

        public async Task<List<FavouriteProduct>> GetFavoritesAsync(string userId)
        {
            var user = await _userService.GetUserOrThrowAsyncId(userId);

            var favorites = await _favoriteRepository.GetFavoritesAsync(user);
            if (favorites == null || favorites.Count == 0)
            {
                return new List<FavouriteProduct>();
            }
            return favorites;
        }

        public async Task RemoveFromFavoritesAsync(int productId, string userId)
        {
            var user = await _userService.GetUserOrThrowAsyncId(userId);

            var product = await _productService.GetProductOrThrowIdAsync(productId);

            var removeFavorite = await _favoriteRepository.GetFavoriteByProductIdAsync(productId, user);
            if (removeFavorite == null)
            {
                throw new ArgumentException("Product is not in favorites", nameof(productId));
            }
            await _favoriteRepository.RemoveFromFavoritesAsync(removeFavorite);
        }
    }
}
