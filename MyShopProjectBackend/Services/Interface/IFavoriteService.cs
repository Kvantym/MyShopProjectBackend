using MyShopProjectBackend.Entities;
using MyShopProjectBackend.Models.Favorit;

namespace MyShopProjectBackend.Servises.Interface
{
    public interface IFavoriteService
    {
        public Task AddToFavoritesAsync(AddFavoritModel model, string userId);
        public Task RemoveFromFavoritesAsync(int productId, string userId);
        public Task <List<FavouriteProduct>> GetFavoritesAsync(string userId);
    }
}
