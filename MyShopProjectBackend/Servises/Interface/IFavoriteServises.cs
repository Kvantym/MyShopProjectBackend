using MyShopProjectBackend.Models;
using MyShopProjectBackend.ViewModels.Add;
using MyShopProjectBackend.ViewModels.Remove;

namespace MyShopProjectBackend.Servises.Interface
{
    public interface IFavoriteServises
    {
        public Task AddToFavoritesAsync(AddFavoritModel model);
        public Task RemoveFromFavoritesAsync(RemoveFavoritModel model);
        public Task <List<FavouriteProduct>> GetFavoritesAsync(string userId);
    }
}
