using Microsoft.EntityFrameworkCore;
using MyShopProjectBackend.Db;
using MyShopProjectBackend.Exceptions;
using MyShopProjectBackend.Models;
using MyShopProjectBackend.Servises.Interface;
using MyShopProjectBackend.ViewModels.Add;
using MyShopProjectBackend.ViewModels.Remove;

namespace MyShopProjectBackend.Servises
{
    public class FavoriteServises : IFavoriteServises
    {
        private readonly AppDbConection _context;

        public FavoriteServises(AppDbConection context)
        {
            _context = context;
        }
        public async Task AddToFavoritesAsync(AddFavoritModel model)
        {
            var product = await _context.products.FindAsync(model.ProductId);

            if (product == null)
            {
                throw new BadRequestException($"Товар з ID{model.ProductId} не знайдено"); 
            }

            bool alreadyExists = await _context.favoritProducts.AnyAsync(fp => fp.UserId == model.UserId && fp.ProductId == model.ProductId);

            if (alreadyExists)
            {
                throw new BadRequestException("Товар вже додано до обраного");
            }

            var favoritProduct = new FavouriteProduct
            {
                UserId = model.UserId,
                ProductId = model.ProductId,
            };

            _context.favoritProducts.Add(favoritProduct);
            await _context.SaveChangesAsync();
        }

        public async Task<List<FavouriteProduct>> GetFavoritesAsync(string userId)
        {
            var favoritProducts = await _context.favoritProducts.Include(fp => fp.Product).Where(fp => fp.UserId == userId).ToListAsync();

            if (!favoritProducts.Any())
            {
                throw new NotFoundException("Немає улюблених продуктів для цього користувача");
            }
            return favoritProducts;
        }

        public async Task RemoveFromFavoritesAsync(RemoveFavoritModel model)
        {
            var product = await _context.products.FindAsync(model.ProductId);

            if (product == null)
            {
                throw new BadRequestException($"Товар з ID {model.ProductId} не знайдено");
            }

            var favoritProduct = await _context.favoritProducts.FirstOrDefaultAsync(fp => fp.UserId == model.UserId && fp.ProductId == model.ProductId);

            if (favoritProduct == null)
            {
                throw new NotFoundException("Цей товар не знайдено в обраному");
            }

            _context.favoritProducts.Remove(favoritProduct);
            await _context.SaveChangesAsync();
        }
    }
}
