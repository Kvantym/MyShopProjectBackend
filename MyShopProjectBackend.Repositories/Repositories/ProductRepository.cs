using Microsoft.EntityFrameworkCore;
using MyShopProjectBackend.Domain.Entities;
using MyShopProjectBackend.Infrastructure;
using MyShopProjectBackend.Repositories.Interfaces;

namespace MyShopProjectBackend.Repositories.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbConect _context;
        public ProductRepository(AppDbConect context)
        {
            _context = context;
        }
        public async Task AddProductAsync(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateProductAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteProductAsync(Product product)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }
        public async Task<List<Product>> GetProductsByNameAsync(string productName)
        {
            return await _context.Products
                .Where(p => p.Name.Contains(productName, StringComparison.OrdinalIgnoreCase))
                .ToListAsync();
        }
        public async Task<List<Product>> GetProductsByShopAsync(Shop shop)
        {
            return await _context.Products
                .Where(p => p.Shop.Id == shop.Id)
                .ToListAsync();
        }
        public async Task<Product> GetProductAsync(Product product)
        {
            return await _context.Products.FindAsync(product.Id);
        }
        public async Task<List<Product>> GetProductsAsync()
        {
            return await _context.Products.ToListAsync();
        }
        public async Task<List<Product>> SearchProductsByNameAsync(string name)
        {
            return await _context.Products
                .Where(p => p.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
                .ToListAsync();
        }
    }
}
