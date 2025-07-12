using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyShopProjectBackend.Db;
using MyShopProjectBackend.DTO;
using MyShopProjectBackend.Entities;
using MyShopProjectBackend.Exceptions;
using MyShopProjectBackend.Models;
using MyShopProjectBackend.Models.Product;
using MyShopProjectBackend.Servises.Interface;
using NLog;
using ILogger = NLog.ILogger;

namespace MyShopProjectBackend.Servises
{
    public class ProductServises : IProductServises
    {
        private readonly AppDbConection _context;
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly UserManager<ApplicationUser> _userManager;

        public ProductServises(AppDbConection conection, UserManager<ApplicationUser> userManager)
        {
            _context = conection;
            _userManager = userManager;
        }

        public async Task AddProductAsync(CreateProductModel model, string ownerId)
        {
            _logger.Info($"{nameof(AddProductAsync)}: Виклик методу");
            var shop = await _context.shops.FindAsync(model.ShopId);
            if (shop == null)
            {
                _logger.Warn($"{nameof(AddProductAsync)}: Магазин з ID {model.ShopId} не знайдено");
                throw new NotFoundException("Магазин не знайдено");
            }
            var seller = await _userManager.FindByIdAsync(ownerId);
            if (seller == null)
            {
                _logger.Warn($"{nameof(AddProductAsync)}: Користувача з ID {ownerId} не знайдено");
                throw new NotFoundException("Користувач не знайдений");
            }
            if (shop.OwnerId != seller.Id)
            {
                _logger.Warn($"{nameof(AddProductAsync)}: Користувач {seller.UserName} не має прав на магазин {shop.Name}");
                throw new AuthorizationException("Ви не маєте права додавати товар до чужого магазину");
            }

            var product = new Product
            {
                Name = model.Name,
                Description = model.Description,
                Category = model.Category,
                Price = model.Price,
                ShopId = shop.Id,
                Quantity = model.Quantity,
                ImageData = model.ImageData,
                ImageMimeType = model.ImageMimeType,
            };

            await _context.products.AddAsync(product);
            await _context.SaveChangesAsync();

            _logger.Info($"{nameof(AddProductAsync)}: Товар '{product.Name}' додано до магазину ID {shop.Name}, {nameof(AddProductAsync)}: Успішно виконано");
        }

        public async Task DeleteProductAsync(int productId, string ownerId)
        {
            _logger.Info($"{nameof(DeleteProductAsync)}: Виклик методу");
            var product = await _context.products.FindAsync(productId);
            if (product == null)
            {
                _logger.Warn($"{nameof(DeleteProductAsync)}: Товар з ID {productId} не знайдено");
                throw new NotFoundException("Товар не знайдено");
            }
            var seller = await _userManager.FindByIdAsync(ownerId);
            if (seller == null)
            {
                _logger.Warn($"{nameof(DeleteProductAsync)}: Користувача з ID {ownerId} не знайдено");
                throw new NotFoundException("Користувач не знайдений");
            }
            var shop = await _context.shops.FindAsync(product.ShopId);
            if (shop == null)
            {
                _logger.Warn($"{nameof(DeleteProductAsync)}: Магазин з ID {product.ShopId} не знайдено");
                throw new NotFoundException("Магазин товару не знайдено");
            }
            if (shop.OwnerId != seller.Id)
            {
                _logger.Warn($"{nameof(DeleteProductAsync)}: Користувач {seller.UserName} не має прав на видалення товару з магазину {shop.Name}");
                throw new AuthorizationException("Ви не маєте права видаляти товар з чужого магазину");
            }

            _context.products.Remove(product);
            await _context.SaveChangesAsync();

            _logger.Info($"{nameof(DeleteProductAsync)}: Товар '{product.Name}' видалено з магазину ID {shop.Name}, {nameof(DeleteProductAsync)}: Виконано успішно");
        }

        public async Task<List<ProductDto>> GetProductByNameAsync(string productName)
        {
            _logger.Info($"{nameof(GetProductByNameAsync)}: Виклик методу");
            var products = await _context.products
               .Where(p => p.Name == productName)
               .Select(p => new ProductDto
               {
                   Id = p.Id,
                   Name = p.Name,
                   Description = p.Description,
                   Category = p.Category,
                   Price = p.Price,
                   ShopId = p.ShopId,
                   Quantity = p.Quantity,
                   ImageData = p.ImageData,
                   ImageMimeType = p.ImageMimeType
               })
               .ToListAsync();

            if (products == null || products.Count == 0)
            {
                _logger.Warn($"{nameof(GetProductByNameAsync)}: Товари з назвою '{productName}' не знайдено");
                throw new NotFoundException("Товари не знайдено з таким іменем");
            }

            _logger.Info($"{nameof(GetProductByNameAsync)}: Знайдено {products.Count} товарів з назвою '{productName}', {nameof(GetProductByNameAsync)}: Виконано успішно");
            return products;
        }

        public async Task<List<ProductDto>> GetProductsByShopAsync(int shopId)
        {
            _logger.Info($"{nameof(GetProductsByShopAsync)}: Виклик методу");
            var shop = await _context.shops.FindAsync(shopId);
            if (shop == null)
            {
                _logger.Warn($"{nameof(GetProductsByShopAsync)}: Магазин з ID {shopId} не знайдено");
                throw new NotFoundException("Магазин не знайдено");
            }

            var products = await _context.products.Where(p => p.ShopId == shopId).Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Category = p.Category,
                Price = p.Price,
                ShopId = p.ShopId,
                Quantity = p.Quantity,
                ImageData = p.ImageData,
                ImageMimeType = p.ImageMimeType
            }).ToListAsync();

            _logger.Info($"{nameof(GetProductsByShopAsync)}: Знайдено {products.Count} товарів у магазині  {shop.Name}, {nameof(GetProductsByShopAsync)}: Виконано успішно");
            return products;
        }

        public async Task UpdateProductAsync(UpdateProductModel model, string ownerId)
        {
            _logger.Info($"{nameof(UpdateProductAsync)}: Виклик методу");
            var product = await _context.products.FindAsync(model.ProductId);
            if (product == null)
            {
                _logger.Warn($"{nameof(UpdateProductAsync)}: Товар з ID {model.ProductId} не знайдено");
                throw new NotFoundException("Товар не знайдено");
            }
            var seller = await _userManager.FindByIdAsync(ownerId);
            if (seller == null) {
                _logger.Warn($"{nameof(DeleteProductAsync)}: Користувача з ID {ownerId} не знайдено");
                throw new NotFoundException("Користувач не знайдений");
            }
            var shop = await _context.shops.FindAsync(product.ShopId);
            if (shop == null)
            {
                _logger.Warn($"{nameof(UpdateProductAsync)}: Магазин для товару ID {product.Id} не знайдено");
                throw new NotFoundException("Магазин товару не знайдено");
            }
            if (shop.OwnerId != seller.Id)
            {
                _logger.Warn($"{nameof(UpdateProductAsync)}: Користувач {seller.UserName} не має прав на редагування товару з магазину {shop.Name}");
                throw new AuthorizationException("Ви не маєте права редагувати товар з чужого магазину");
            }

            product.Name = model.Name;
            product.Description = model.Description;
            product.Category = model.Category;
            product.Price = model.Price;
            product.Quantity = model.Quantity;
            product.ImageData = model.ImageData;
            product.ImageMimeType = model.ImageMimeType;

            await _context.SaveChangesAsync();

            _logger.Info($"{nameof(UpdateProductAsync)}: Товар {product.Name} успішно оновлено");
        }
    }
}
