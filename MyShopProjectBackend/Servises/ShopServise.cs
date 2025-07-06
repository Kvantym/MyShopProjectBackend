using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyShopProjectBackend.Db;
using MyShopProjectBackend.DTO;
using MyShopProjectBackend.Exceptions;
using MyShopProjectBackend.Models;
using MyShopProjectBackend.Servises.Interface;
using MyShopProjectBackend.ViewModels.Create;
using MyShopProjectBackend.ViewModels.Delete;
using MyShopProjectBackend.ViewModels.Update;

namespace MyShopProjectBackend.Servises
{
    public class ShopServise : IShopServise
    {
     private readonly AppDbConection _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ShopServise(AppDbConection context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task CreateShopAsync(CreateShopModel model)
        {
            var user = await _userManager.FindByIdAsync(model.OwnerId.ToString());

            if (user == null)
            {
                throw new NotFoundException("Користувача не знайдено");
            }

            var isSeller = await _userManager.IsInRoleAsync(user, "Seller");
            if (!isSeller)
            {
                throw new AuthorizationException("Ви не зареєстровані як продавець");
            }

            var shop = new Models.Shop
            {
                Name = model.Name,
                Description = model.Description,
                OwnerId = user.Id
            };

            await _context.shops.AddAsync(shop);
            await _context.SaveChangesAsync();
        }


        public async Task DeleteShopAsync(DeleteShopModel model)
        {
            var shop = await _context.shops.FindAsync(model.ShopId);
            if (shop == null)
            {
                throw new NotFoundException("Магазин не знайдено");
            }

            if (shop.OwnerId != model.OwnerId)
            {
                throw new BadRequestException("Ви не маєте права видалити цей магазин");
            }

            _context.shops.Remove(shop);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ShopDto>> GetAllShopsAsync(string OwnerId)
        {
            var shops = await _context.shops.Where(s => s.OwnerId == OwnerId).ToListAsync();
            if (shops == null || !shops.Any())
            {
                throw new NotFoundException("Магазини не знайдено");
            }

            var shopDtos = shops.Select(s => new ShopDto
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
                
            }).ToList();

            return shopDtos;
        }

        public async Task<ShopDto?> GetShopByIdAsync(int shopId)
        {
            var shop = await _context.shops.FindAsync(shopId);

            if (shop == null)
            {
                throw new NotFoundException("Магазин не знайдено");
            }
            var shopDto = new ShopDto
            {
                Id = shop.Id,
                Name = shop.Name,
                Description = shop.Description,
            };

            return shopDto ;
        }

        public async Task UpdateShopAsync(UpdateShopModel model)
        {
            var shop = await _context.shops.FindAsync(model.ShopId);

            if (shop == null)
            {
                throw new NotFoundException("Магазин не знайдено");
            }

            if (shop.OwnerId != model.OwnerId)
            {
                throw new BadRequestException("Ви не маєте прав змінювати цей магазин");
            }

            shop.Name = model.Name;
            shop.Description = model.Description;

            await _context.SaveChangesAsync();
        }
    }
}
