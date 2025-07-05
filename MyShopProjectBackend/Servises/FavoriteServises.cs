using Microsoft.EntityFrameworkCore;
using MyShopProjectBackend.Db;
using MyShopProjectBackend.Models;
using MyShopProjectBackend.Servises.Interface;
using MyShopProjectBackend.ViewModels;

namespace MyShopProjectBackend.Servises
{
    public class FavoriteServises : IFavoriteServises
    {
        private readonly AppDbConection _context;

        public FavoriteServises(AppDbConection context)
        {
            _context = context;
        }
        public async Task<(bool Success, string? ErrorMessage)> AddToFavoritesAsync(AddFavoritModel model)
        {
            var product = await _context.products.FindAsync(model.ProductId);

            if (product == null)
            {
                return (false,"Товару не існує");
            }

            bool alreadyExists = await _context.favoritProducts.AnyAsync(fp => fp.UserId == model.UserId && fp.ProductId == model.ProductId);

            if (alreadyExists)
            {
                return (false, "Товар вже додано до обраного");
            }

            var favoritProduct = new FavouriteProduct
            {
                UserId = model.UserId,
                ProductId = model.ProductId,
            };

            _context.favoritProducts.Add(favoritProduct);
            await _context.SaveChangesAsync();

            return (true,null);
        }

        public async Task<(bool Success, string? ErrorMessage, List<FavouriteProduct> FavoriteProducts)> GetFavoritesAsync(int userId)
        {
            var favoritProducts = await _context.favoritProducts.Include(fp => fp.Product).Where(fp => fp.UserId == userId).ToListAsync();

            if (!favoritProducts.Any())
            {
                return (false, "Немає улюблених продуктів", new List<FavouriteProduct>());
            }
            return (true, null, favoritProducts);
        }

        public async Task<(bool Success, string? ErrorMessage)> RemoveFromFavoritesAsync(RemoveFavoritModel model)
        {
            var product = await _context.products.FindAsync(model.ProductId);

            if (product == null)
            {
                return  (false,"Неправельне ID товару");
            }

            var favoritProduct = await _context.favoritProducts.FirstOrDefaultAsync(fp => fp.UserId == model.UserId && fp.ProductId == model.ProductId);

            if (favoritProduct == null)
            {
                return (false, "Цей товар не знайдено в обраному");
            }

            _context.favoritProducts.Remove(favoritProduct);
            await _context.SaveChangesAsync();

            return (true,null);
        }
    }
}
