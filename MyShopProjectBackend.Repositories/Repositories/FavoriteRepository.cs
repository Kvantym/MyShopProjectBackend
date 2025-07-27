using Microsoft.EntityFrameworkCore;
using MyShopProjectBackend.Domain.Entities;
using MyShopProjectBackend.Infrastructure;
using MyShopProjectBackend.Repositories.Interfaces;

namespace MyShopProjectBackend.Repositories.Repositories
{
    public class FavoriteRepository : IFavoriteRepository
    {
        private readonly AppDbConect _context;
        public FavoriteRepository(AppDbConect context)
        {
            _context = context;
        }
        public async Task AddToFavoritesAsync(FavouriteProduct favorite)
        {
          await _context.FavoritProducts.AddAsync(favorite);
          await _context.SaveChangesAsync();
        }

        public async Task<List<FavouriteProduct>> GetFavoritesAsync(ApplicationUser user)
        {
           return await _context.FavoritProducts
                .Where(fp => fp.UserId == user.Id)
                .Include(fp => fp.Product)
                .ToListAsync();
        }

        public async Task RemoveFromFavoritesAsync(FavouriteProduct product)
        {
             _context.FavoritProducts.Remove(product);
             await _context.SaveChangesAsync();
        }
       public async Task<FavouriteProduct> GetFavoriteByProductIdAsync(int productId, ApplicationUser user)
        {
            return await _context.FavoritProducts
                .FirstOrDefaultAsync(fp => fp.ProductId == productId && fp.UserId == user.Id);
        }
    }
}
