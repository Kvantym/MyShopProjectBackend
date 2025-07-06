using MyShopProjectBackend.DTO;
using MyShopProjectBackend.ViewModels.Create;
using MyShopProjectBackend.ViewModels.Delete;
using MyShopProjectBackend.ViewModels.Update;

namespace MyShopProjectBackend.Servises.Interface
{
    public interface IProductServises
    {
        public Task AddProductAsync(CreateProductModel model);
        public Task UpdateProductAsync(UpdateProductModel model);
        public Task DeleteProductAsync(DeleteProductModel model);
        public Task<List<ProductDto>> GetProductByNameAsync(string productName);
        public Task<List<ProductDto>> GetProductsByShopAsync(int shopId);

    }
}
