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
    
    public class ProductServises : IProductServises
    {
        private readonly AppDbConection _context;

        public ProductServises(AppDbConection conection)
        {
            _context = conection;
        }
        public async Task AddProductAsync(CreateProductModel model)
        {
            var shop = await _context.shops.FindAsync(model.ShopId);

            if (shop == null)
            {
                throw new NotFoundException("Магазин не знайдено");
            }

            if (shop.OwnerId != model.OwnerId)
            {
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
        }

        public async Task DeleteProductAsync(DeleteProductModel model)
        {
            var product = await _context.products.FindAsync(model.ProductId);

            if (product == null) 
            {
                throw new NotFoundException("Товар не знайдено");
            }

            var shop = await _context.shops.FindAsync(product.ShopId);
            if (shop == null)
            {
                throw new NotFoundException("Магазин товару не знайдено");
            }

            if (shop.OwnerId != model.OwnerId)
            {
                throw new NotFoundException("Ви не маєте права видаляти товар з чужого магазину");
            }
            _context.products.Remove(product);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ProductDto>> GetProductByNameAsync(string productName)
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

            if (products == null || !products.Any())
            {
                throw new NotFoundException("Товари не знайдено з таким іменем");
            }
            return products;
        }


        public async Task<List<ProductDto>> GetProductsByShopAsync(int shopId)
        {
            var shop = await _context.shops.FindAsync(shopId);

            if (shop == null)
            {
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

            return products;
        }

        public async Task UpdateProductAsync(UpdateProductModel model)
        {
            var product = await _context.products.FindAsync(model.ProductId);// Отримання товару за його ID
            if (product == null)
            {
                throw new NotFoundException("Товар не знайдено");
            }
            var shop = await _context.shops.FindAsync(product.ShopId);// Отримання магазину, до якого належить товар
            if (shop == null)
            {
                throw new NotFoundException("Магазин товару не знайдено");
            }

            if (shop.OwnerId != model.OwnerId)
            {
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
        }
    }
}
