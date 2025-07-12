using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyShopProjectBackend.Db;
using MyShopProjectBackend.Entities;
using MyShopProjectBackend.Exceptions;
using MyShopProjectBackend.Models.Favorit;
using MyShopProjectBackend.Servises.Interface;
using NLog;
using ILogger = NLog.ILogger;

namespace MyShopProjectBackend.Servises
{
    public class FavoriteServises : IFavoriteServises
    {
        private readonly AppDbConection _context;
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly UserManager<ApplicationUser> _userManager;

        public FavoriteServises(AppDbConection context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task AddToFavoritesAsync(AddFavoritModel model , string userId)
        {
            _logger.Info($"{nameof(AddToFavoritesAsync)}: Виклик методу");
          
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.Warn($"{nameof(AddToFavoritesAsync)}: Користувача з ID {userId} не знайдено");
                throw new NotFoundException("Користувач не знайдений");
            }

            var product = await _context.products.FindAsync(model.ProductId);
            if (product == null)
            {
                _logger.Warn($"{nameof(AddToFavoritesAsync)}: Товар з ID {model.ProductId} не знайдено");
                throw new BadRequestException($"Товар з ID {model.ProductId} не знайдено");
            }
            _logger.Info($"{nameof(AddToFavoritesAsync)}: Додавання товару з ID {product.Name} до обраного користувача з ID {user.UserName}");
            bool alreadyExists = await _context.favoritProducts.AnyAsync(fp => fp.UserId == userId && fp.ProductId == model.ProductId);
            if (alreadyExists)
            {
                _logger.Warn($"{nameof(AddToFavoritesAsync)}: Товар з ID {model.ProductId} вже додано до обраного користувача з ID {user.UserName}");
                throw new BadRequestException("Товар вже додано до обраного");
            }

            var favoritProduct = new FavouriteProduct
            {
                UserId = userId,
                ProductId = model.ProductId,
            };
            _context.favoritProducts.Add(favoritProduct);
            await _context.SaveChangesAsync();

            _logger.Info($"{nameof(AddToFavoritesAsync)}: Товар з ID {model.ProductId} успішно додано до обраного користувача  {user.UserName}");
        }

        public async Task<List<FavouriteProduct>> GetFavoritesAsync(string userId)
        {
            _logger.Info($"{nameof(GetFavoritesAsync)}: Виклик методу");
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) 
            {
                _logger.Warn($"{nameof(AddToFavoritesAsync)}: Користувача з ID {userId} не знайдено");
                throw new NotFoundException("Користувач не знайдений");
            }

            _logger.Info($"{nameof(GetFavoritesAsync)}: Отримання улюблених товарів для користувача з ID {user.UserName}");

            var favoritProducts = await _context.favoritProducts.Include(fp => fp.Product)
                .Where(fp => fp.UserId == userId)
                .ToListAsync();

            if (favoritProducts.Count == 0)
            {
                _logger.Warn($"{nameof(GetFavoritesAsync)}: У користувача з ID {user.UserName} немає улюблених товарів");
                throw new NotFoundException("Немає улюблених продуктів для цього користувача");
            }

            _logger.Info($"{nameof(GetFavoritesAsync)}: Улюблені товари для користувача з ID {user.UserName} успішно отримано");
            return favoritProducts;
        }

        public async Task RemoveFromFavoritesAsync(int productId, string userId)
        {
            _logger.Info($"{nameof(RemoveFromFavoritesAsync)}: Виклик методу");
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.Info($"{nameof(RemoveFromFavoritesAsync)}: Користувач {userId} не знайдений");
                throw new RegistrationException("Користувач з таким іменем не знайдений");
            }

            var product = await _context.products.FindAsync(productId);
            if (product == null)
            {
                _logger.Warn($"{nameof(RemoveFromFavoritesAsync)}: Товар з ID {productId} не знайдено");
                throw new BadRequestException($"Товар з ID {productId} не знайдено");
            }
            _logger.Info($"{nameof(RemoveFromFavoritesAsync)}: Видалення товару з ID {product.Name} з обраного користувача з ID {user.UserName}");

            var favoritProduct = await _context.favoritProducts
                .FirstOrDefaultAsync(fp => fp.UserId == userId && fp.ProductId == productId);

            if (favoritProduct == null)
            {
                _logger.Warn($"{nameof(RemoveFromFavoritesAsync)}: Товар з ID {product.Name} не знайдено в обраному користувача з ID {user.UserName}");
                throw new NotFoundException("Цей товар не знайдено в обраному");
            }

            _context.favoritProducts.Remove(favoritProduct);
            await _context.SaveChangesAsync();

            _logger.Info($"{nameof(RemoveFromFavoritesAsync)}: Товар з ID {product.Name} успішно видалено з обраного користувача з ID {user.UserName}");
        }
    }
}
