using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyShopProjectBackend.Db;
using MyShopProjectBackend.DTO;
using MyShopProjectBackend.Models;
using MyShopProjectBackend.Servises.Interface;
using MyShopProjectBackend.ViewModels;
using System.Security.Claims;

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
        public async Task<(bool Success, string? ErrorMessage)> CreateShopAsync(CreateShopModel model)
        {
            var user = await _userManager.FindByIdAsync(model.OwnerId.ToString());

            if (user == null)
            {
                return (false, "Користувача не знайдено");
            }

            // Перевірка, чи користувач має роль "Seller"
            var isSeller = await _userManager.IsInRoleAsync(user, "Seller");
            if (!isSeller)
            {
                return (false, "Ви не зареєстровані як продавець");
            }

            var shop = new Models.Shop
            {
                Name = model.Name,
                Description = model.Description,
                OwnerId = int.Parse(user.Id)
            };

            await _context.shops.AddAsync(shop);
            await _context.SaveChangesAsync();

            return (true, null);
        }


        public async Task<(bool Success, string? ErrorMessage)> DeleteShopAsync(DeleteShopModel model)
        {
            var shop = await _context.shops.FindAsync(model.ShopId);
            if (shop == null)
            {
                return (false, "Магазин не знайдено");
            }

            if (shop.OwnerId != model.OwnerId)
            {
                return (false,"Ви не маєте права видалити цей магазин");
            }

            _context.shops.Remove(shop);
            await _context.SaveChangesAsync();

            return (true,null);
        }

        public async Task<(bool Success, string? ErrorMessage, List<ShopDto> Shops)> GetAllShopsAsync(int OwnerId)
        {
            var shops = await _context.shops.Where(s => s.OwnerId == OwnerId).ToListAsync();
            if (shops == null || !shops.Any())
            {
                return (false, "Магазини не знайдено", new List<ShopDto>());
            }

            var shopDtos = shops.Select(s => new ShopDto
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
                
            }).ToList();

            return (true, null, shopDtos);
        }

        public async Task<(bool Success, string? ErrorMessage, ShopDto? Shop)> GetShopByIdAsync(int shopId)
        {
            var shop = await _context.shops.FindAsync(shopId);

            if (shop == null)
            {
                return (false,"Магазин не знайдено", null);
            }
            var shopDto = new ShopDto
            {
                Id = shop.Id,
                Name = shop.Name,
                Description = shop.Description,
            };

            return (true,null, shopDto) ;
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateShopAsync(UpdateShopModel model)
        {
            var shop = await _context.shops.FindAsync(model.ShopId);

            if (shop == null)
            {
                return (false, "Магазин не знайдено");
            }

            if (shop.OwnerId != model.OwnerId)
            {
                return (false,"Ви не маєте прав змінювати цей магазин");
            }

            shop.Name = model.Name;
            shop.Description = model.Description;

            await _context.SaveChangesAsync();

            return (true,null);
        }
    }
}
