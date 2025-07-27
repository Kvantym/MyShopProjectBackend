using MyShopProjectBackend.Domain.Entities;

namespace MyShopProjectBackend.Repositories.Interfaces
{
    public interface IProductRepository
    {
        Task AddProductAsync(Product product);
        Task UpdateProductAsync(Product product);
        Task DeleteProductAsync(Product product);
        Task<List<Product>> GetProductsByNameAsync(string productName);
        Task<List<Product>> GetProductsByShopAsync(Shop shop);
        Task<Product> GetProductAsync(int productId);
        Task<List<Product>> GetProductsAsync();
        Task<List<Product>> SearchProductsByNameAsync(string name);
        Task<Product> GetProductByNameAsync(string productName, Shop shop);
    }
}
