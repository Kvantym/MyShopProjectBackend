using MyShopProjectBackend.DTO;
using MyShopProjectBackend.ViewModels;

namespace MyShopProjectBackend.Servises.Interface
{
    public interface IProductServises
    {
        public Task<(bool Success, string? ErrorMessage)> AddProductAsync(CreateProductModel model);
        public Task<(bool Success, string? ErrorMessage)> UpdateProductAsync(UpdateProductModel model);
        public Task<(bool Success, string? ErrorMessage)> DeleteProductAsync(DeleteProductModel model);
        public Task<(bool Success, string? ErrorMessage, List<ProductDto> Products)> GetProductByNameAsync(string productName);
        public Task<(bool Success, string? ErrorMessage, List<ProductDto> Products)> GetProductsByShopAsync(int shopId);

    }
}
