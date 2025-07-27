using MyShopProjectBackend.Domain.Entities;
using MyShopProjectBackend.Domain.Request.Favorite;

namespace MyShopProjectBackend.Services.Interface
{
    public interface IFavoriteService
    {
        public Task AddToFavoritesAsync(AddFavoritRequest request, string userId);
        public Task RemoveFromFavoritesAsync(int productId, string userId);
        public Task<List<FavouriteProduct>> GetFavoritesAsync(string userId);
    }
}
