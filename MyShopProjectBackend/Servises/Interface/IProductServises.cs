using MyShopProjectBackend.DTO;
using MyShopProjectBackend.Entities;
using MyShopProjectBackend.Models.Product;

namespace MyShopProjectBackend.Servises.Interface
{
    public interface IProductServises
    {
        public Task AddProductAsync(CreateProductModel model, string ownerId);
        public Task UpdateProductAsync(UpdateProductModel model, string ownerId);
        public Task DeleteProductAsync(int productId, string ownerId);
        public Task<List<ProductDto>> GetProductsByNameAsync(string productName);
        public Task<List<ProductDto>> GetProductsByShopAsync(int shopId);
        public Task<Product> GetProductOrThrowIdAsync(int productId);

    }
}
