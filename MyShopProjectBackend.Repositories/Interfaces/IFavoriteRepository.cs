using MyShopProjectBackend.Domain.Entities;

namespace MyShopProjectBackend.Repositories.Interfaces
{
    public interface IFavoriteRepository
    {
        Task AddToFavoritesAsync(FavouriteProduct favorite);
        Task RemoveFromFavoritesAsync(FavouriteProduct product);
        Task<List<FavouriteProduct>> GetFavoritesAsync(ApplicationUser user);

    }
}
