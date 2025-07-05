using MyShopProjectBackend.Models;
using MyShopProjectBackend.ViewModels;

namespace MyShopProjectBackend.Servises.Interface
{
    public interface IFavoriteServises
    {
        public Task<(bool Success, string? ErrorMessage)> AddToFavoritesAsync(AddFavoritModel model);
        public Task<(bool Success, string? ErrorMessage)> RemoveFromFavoritesAsync(RemoveFavoritModel model);
        public Task<(bool Success, string? ErrorMessage, List<FavouriteProduct> FavoriteProducts)> GetFavoritesAsync(int userId);
    }
}
