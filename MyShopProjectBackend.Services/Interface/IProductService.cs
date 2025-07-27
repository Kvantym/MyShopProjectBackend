using MyShopProjectBackend.Domain.Entities;
using MyShopProjectBackend.Domain.Request.Product;
using MyShopProjectBackend.Domain.Responses;

namespace MyShopProjectBackend.Services.Interface
{
    public interface IProductService
    {
        public Task AddProductAsync(AddProductRequest request, string ownerId);
        public Task UpdateProductAsync(UpdateProductRequest request, string ownerId);
        public Task DeleteProductAsync(int productId, string ownerId);
        public Task<List<ProductResponse>> GetProductsByNameAsync(string productName);
        public Task<List<ProductResponse>> GetProductsByShopAsync(int shopId);
        public Task<Product> GetProductOrThrowIdAsync(int productId);
        public Task<List<ProductResponse>> GetProductsAsync();
        public Task<List<ProductResponse>> SearchProductsByNameAsync(string name);
        public Task<ProductResponse> GetProductByIdAsync(int productId);
    }
}
