using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyShopProjectBackend.Db;
using MyShopProjectBackend.DTO;
using MyShopProjectBackend.Entities;
using MyShopProjectBackend.Exceptions;
using MyShopProjectBackend.Models.Shop;
using MyShopProjectBackend.Servises.Interface;
using NLog;

namespace MyShopProjectBackend.Servises
{
    public class ShopServise : IShopServise
    {
        private readonly AppDbConection _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly NLog.ILogger _logger =LogManager.GetCurrentClassLogger();

        public ShopServise(AppDbConection context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task CreateShopAsync(CreateShopModel model, string ownerId)
        {
            _logger.Info($"{nameof(CreateShopAsync)}: Виклик методу");
            var user = await _userManager.FindByIdAsync(ownerId);
            if (user == null)
            {
                _logger.Warn($"{nameof(CreateShopAsync)}: Користувача з ID {ownerId} не знайдено");
                throw new NotFoundException("Користувача не знайдено");
            }

            var isSeller = await _userManager.IsInRoleAsync(user, "Seller");
            if (!isSeller)
            {
                _logger.Warn($"{nameof(CreateShopAsync)}: Користувач {user.UserName} не є продавцем");
                throw new AuthorizationException("Ви не зареєстровані як продавець");
            }

            var shop = new Shop
            {
                Name = model.Name,
                Description = model.Description,
                OwnerId = user.Id
            };

            await _context.shops.AddAsync(shop);
            await _context.SaveChangesAsync();

            _logger.Info($"{nameof(CreateShopAsync)}: Магазин '{shop.Name}' успішно створено для користувача {user.UserName}, {nameof(CreateShopAsync)}: Успішно виконано");
        }

        public async Task DeleteShopAsync(int shopId, string ownerId)
        {
            _logger.Info($"{nameof(DeleteShopAsync)}: Виклик методу");
            var shop = await _context.shops.FindAsync(shopId);
            if (shop == null)
            {
                _logger.Warn($"{nameof(DeleteShopAsync)}: Магазин з ID {shopId} не знайдено");
                throw new NotFoundException("Магазин не знайдено");
            }
            var user = await _userManager.FindByIdAsync(ownerId);
            if (user == null)
            {
              _logger.Warn($"{nameof(DeleteShopAsync)}: Користувача з ID {ownerId} не знайдено");
               throw new NotFoundException("Користувача не знайдено");
            }

            if (shop.OwnerId != ownerId)
            {
                _logger.Warn($"{nameof(DeleteShopAsync)}: Користувач з ID {user.UserName} не має права видаляти магазин з ID {shop.Name}");
                throw new BadRequestException("Ви не маєте права видалити цей магазин");
            }

            _context.shops.Remove(shop);
            await _context.SaveChangesAsync();

            _logger.Info($"{nameof(DeleteShopAsync)}: Магазин з ID {shop.Name} успішно видалено, {nameof(DeleteShopAsync)}: Успішно виконано");
        }

        public async Task<List<ShopDto>> GetAllShopsAsync(string OwnerId)
        {
            _logger.Info($"{nameof(GetAllShopsAsync)}: Виклик методу");
            var shops = await _context.shops.Where(s => s.OwnerId == OwnerId).ToListAsync();
            var user = await _userManager.FindByIdAsync(OwnerId);
            if (user == null)
            {
                _logger.Warn($"{nameof(GetAllShopsAsync)}: Користувача з ID {OwnerId} не знайдено");
                throw new NotFoundException("Користувача не знайдено");
            }

            if (shops == null || !shops.Any())
            {
                _logger.Warn($"{nameof(GetAllShopsAsync)}: Магазини для власника з ID {user.UserName} не знайдено");
                throw new NotFoundException("Магазини не знайдено");
            }

            _logger.Info($"{nameof(GetAllShopsAsync)}: Знайдено {shops.Count} магазинів для власника: {user.UserName}");

            var shopDtos = shops.Select(s => new ShopDto
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
            }).ToList();
            _logger.Info($"{nameof(GetAllShopsAsync)}: Успішно виконано");
            return shopDtos;
        }

        public async Task<ShopDto?> GetShopByIdAsync(int shopId)
        {
            _logger.Info($"{nameof(GetShopByIdAsync)}: Виклик методу");
            var shop = await _context.shops.FindAsync(shopId);

            if (shop == null)
            {
                _logger.Warn($"{nameof(GetShopByIdAsync)}: Магазин з ID {shopId} не знайдено");
                throw new NotFoundException("Магазин не знайдено");
            }

            _logger.Info($"{nameof(GetShopByIdAsync)}: Магазин з ID {shop.Name} знайдено");
          var shopDto=   new ShopDto
            {
                Id = shop.Id,
                Name = shop.Name,
                Description = shop.Description,
            };
            _logger.Info($"{nameof(GetShopByIdAsync)}: Успішно виконано");
            return shopDto;
        }

        public async Task UpdateShopAsync(UpdateShopModel model, string ownerId)
        {
            _logger.Info($"{nameof(UpdateShopAsync)}: Виклик методу");
            var shop = await _context.shops.FindAsync(model.ShopId);

            if (shop == null)
            {
                _logger.Warn($"{nameof(UpdateShopAsync)}: Магазин з ID {model.ShopId} не знайдено");
                throw new NotFoundException("Магазин не знайдено");
            }
            var user = await _userManager.FindByIdAsync(ownerId);
            if (user == null) {
                _logger.Warn($"{nameof(GetAllShopsAsync)}: Користувача з ID {ownerId} не знайдено");
                throw new NotFoundException("Користувача не знайдено");
            }

            if (shop.OwnerId != user.Id)
            {
                _logger.Warn($"{nameof(UpdateShopAsync)}: Користувач з ID {user.UserName} не має прав змінювати магазин з ID {shop.Name}");
                throw new BadRequestException("Ви не маєте прав змінювати цей магазин");
            }

            shop.Name = model.Name;
            shop.Description = model.Description;

            await _context.SaveChangesAsync();

            _logger.Info($"{nameof(UpdateShopAsync)}: Магазин з ID {shop.Name} успішно оновлено, {nameof(UpdateShopAsync)}: Успішно виконано");
        }
    }
}
