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
        private readonly IShopServise _shopServise;
        private readonly IUserServise _userServise;

        public ProductServises(AppDbConection conection, UserManager<ApplicationUser> userManager, IShopServise shopServise,IUserServise userServise)
        {
            _context = conection;
            _shopServise = shopServise;
            _userServise = userServise;
        }

        public async Task AddProductAsync(CreateProductModel model, string ownerId) //готово
        {
            _logger.Info($"{nameof(AddProductAsync)}: Виклик методу");

            var seller = await _userServise.GetUserOrThrowAsyncId(ownerId);

            var shop = await _shopServise.EnsureSellerOwnsShopAsync(seller, model.ShopId);

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

        public async Task DeleteProductAsync(int productId, string ownerId)//готово
        {
            _logger.Info($"{nameof(DeleteProductAsync)}: Виклик методу");

            var product = await GetProductOrThrowIdAsync(productId);

            var seller = await _userServise.GetUserOrThrowAsyncId(ownerId);

            var shop = await _shopServise.EnsureSellerOwnsShopForProductAsync(seller, product);

            _context.products.Remove(product);
            await _context.SaveChangesAsync();

            _logger.Info($"{nameof(DeleteProductAsync)}: Товар '{product.Name}' видалено з магазину ID {shop.Name}, {nameof(DeleteProductAsync)}: Виконано успішно");
        }

        public async Task<List<ProductDto>> GetProductsByNameAsync(string productName)//готово
        {
            _logger.Info($"{nameof(GetProductsByNameAsync)}: Виклик методу");

            var products = await FindProductsByName(productName);

            _logger.Info($"{nameof(GetProductsByNameAsync)}: Знайдено {products.Count} товарів з назвою '{productName}', {nameof(GetProductsByNameAsync)}: Виконано успішно");
            return products;
        }

        public async Task<List<ProductDto>> GetProductsByShopAsync(int shopId)//готово
        {
            _logger.Info($"{nameof(GetProductsByShopAsync)}: Виклик методу");

            var shop = await _shopServise.GetShopOrThrowAsync(shopId);

            var products = await CreateProductDtos(shop);

            _logger.Info($"{nameof(GetProductsByShopAsync)}: Знайдено {products.Count} товарів у магазині  {shop.Name}, {nameof(GetProductsByShopAsync)}: Виконано успішно");
            return products;
        }

        public async Task UpdateProductAsync(UpdateProductModel model, string ownerId)//готово
        {
            _logger.Info($"{nameof(UpdateProductAsync)}: Виклик методу");
            var product = await GetProductOrThrowIdAsync(model.ProductId);

            var seller = await _userServise.GetUserOrThrowAsyncId(ownerId);

            var shop = await _shopServise.EnsureSellerOwnsShopForProductAsync(seller, product);

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
        public async Task<Product> GetProductOrThrowIdAsync(int productId)//готово
        {
            var product = await _context.products.FindAsync(productId);
            if (product == null)
            {
                _logger.Error($"{nameof(GetProductOrThrowIdAsync)}: Товар не знайдено");
                throw new NotFoundException("Товар не знайдено");
            }
            _logger.Info($"{nameof(GetProductOrThrowIdAsync)}: Товар з ID {productId} успішно отримано");
            return product;
        }
        private async Task<List<ProductDto>> CreateProductDtos(Shop shop)
        {
            var products = await _context.products.Where(p => p.ShopId == shop.Id).Select(p => new ProductDto
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
            

            return products;
        }
        private async Task<List<ProductDto>> FindProductsByName(string productName)
        {
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

            if (products.Count == 0)
            {
                _logger.Warn($"{nameof(FindProductsByName)}: Товари з назвою '{productName}' не знайдено");
                throw new NotFoundException("Товари не знайдено з таким іменем");
            }
            return products;
        }
  

    }
}
//208