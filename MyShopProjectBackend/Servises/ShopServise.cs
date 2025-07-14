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
        private readonly NLog.ILogger _logger =LogManager.GetCurrentClassLogger();
        private readonly IUserServise _userServise;



        public ShopServise(AppDbConection context, IUserServise userServise)
        {
            _context = context;
        }

        public async Task CreateShopAsync(CreateShopModel model, string ownerId)//готово
        {
            _logger.Info($"{nameof(CreateShopAsync)}: Виклик методу");

            var seller = await _userServise.GetUserOrThrowAsyncId(ownerId);

            var isSeller =  await _userServise.EnsureUserHasRoleOrThrowAsync(seller, "Seller");

            var shop = new Shop
            {
                Name = model.Name,
                Description = model.Description,
                OwnerId = seller.Id
            };

            await _context.shops.AddAsync(shop);
            await _context.SaveChangesAsync();

            _logger.Info($"{nameof(CreateShopAsync)}: Магазин '{shop.Name}' успішно створено для користувача {seller.UserName}, {nameof(CreateShopAsync)}: Успішно виконано");
        }

        public async Task DeleteShopAsync(int shopId, string ownerId)//готово
        {
            _logger.Info($"{nameof(DeleteShopAsync)}: Виклик методу");

            var user = await _userServise.GetUserOrThrowAsyncId(ownerId);

            var shop = await GetShopOrThrowAsync(shopId);

            if (shop.OwnerId != ownerId)
            {
                _logger.Warn($"{nameof(DeleteShopAsync)}: Користувач з ID {user.UserName} не має права видаляти магазин з ID {shop.Name}");
                throw new BadRequestException("Ви не маєте права видалити цей магазин");
            }

            _context.shops.Remove(shop);
            await _context.SaveChangesAsync();

            _logger.Info($"{nameof(DeleteShopAsync)}: Магазин з ID {shop.Name} успішно видалено, {nameof(DeleteShopAsync)}: Успішно виконано");
        }

        public async Task<List<ShopDto>> GetAllShopsAsync(string OwnerId)//готово
        {
            _logger.Info($"{nameof(GetAllShopsAsync)}: Виклик методу");
            var shops = await _context.shops.Where(s => s.OwnerId == OwnerId).ToListAsync();

            var user = await _userServise.GetUserOrThrowAsyncId(OwnerId);

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

        public async Task<ShopDto?> GetShopByIdAsync(int shopId)//готово
        {
            _logger.Info($"{nameof(GetShopByIdAsync)}: Виклик методу");
            var shop = await GetShopOrThrowAsync(shopId);
  
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

        public async Task UpdateShopAsync(UpdateShopModel model, string ownerId)//готово
        {
            _logger.Info($"{nameof(UpdateShopAsync)}: Виклик методу");
            var user = await _userServise.GetUserOrThrowAsyncId(ownerId);
            var shop = await GetShopOrThrowAsync(model.ShopId);

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
        public async Task<Shop> EnsureSellerOwnsShopAsync(ApplicationUser seller, int shopId)
        {
            var shop = await _context.shops.FindAsync(shopId);
            if (shop == null || shop.OwnerId != seller.Id)
            {
                _logger.Warn($"{nameof(EnsureSellerOwnsShopAsync)}: Магазин не знайдено або продавець не є його власником");
                throw new NotFoundException("Магазин не знайдено або ви не є його власником");
            }
            return shop;
        }
        public async Task<Shop> EnsureSellerOwnsShopForProductAsync(ApplicationUser seller, Product product)
        {
            var shop = await _context.shops.FindAsync(product.ShopId);
            if (shop == null)
            {
                _logger.Warn($"{nameof(EnsureSellerOwnsShopForProductAsync)}: Магазин з ID {product.ShopId} не знайдено");
                throw new NotFoundException("Магазин товару не знайдено");
            }
            if (shop.OwnerId != seller.Id)
            {
                _logger.Warn($"{nameof(EnsureSellerOwnsShopForProductAsync)}: Користувач {seller.UserName} не має прав на видалення товару з магазину {shop.Name}");
                throw new AuthorizationException("Ви не маєте права видаляти товар з чужого магазину");
            }
            return shop;
        }
        public async Task<Shop> GetShopOrThrowAsync(int shopId)
        {
            var shop = await _context.shops.FindAsync(shopId);
            if (shop == null)
            {
                _logger.Warn($"{nameof(GetShopOrThrowAsync)}: Магазин з ID {shopId} не знайдено");
                throw new NotFoundException("Магазин не знайдено");
            }
            return shop;
        }
    }
}
