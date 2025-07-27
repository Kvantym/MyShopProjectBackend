using MyShopProjectBackend.Domain.Entities;
using MyShopProjectBackend.Domain.Request.Product;
using MyShopProjectBackend.Domain.Responses;
using MyShopProjectBackend.Repositories.Interfaces;
using MyShopProjectBackend.Services.Interface;

namespace MyShopProjectBackend.Services.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IUserService _userService;
        private readonly IShopService _shopService;

        public ProductService( IProductRepository productRepository, IUserService userService, IShopService shopService)
        {
            _productRepository = productRepository;
            _userService = userService;
            _shopService = shopService;
        }

        public async Task AddProductAsync(AddProductRequest request, string ownerId)
        {
            var user = await _userService.GetUserOrThrowAsyncId(ownerId);

            var shop = await _shopService.GetShopOrThrowAsync(request.ShopId);

            var product = await _productRepository.GetProductByNameAsync(request.Name, shop);
            if (product != null)
            {
                throw new ArgumentException("Product with this name already exists in the shop", nameof(request.Name));
            }
            var newProduct = new Product
            {
                Name = request.Name,
                Description = request.Description,
                Category = request.Category,
                Price = request.Price,
                Quantity = request.Quantity,
                ImageData = request.ImageData,
                ImageMimeType = request.ImageMimeType,
                ShopId = shop.Id,
                OwnerId = user.Id
            };
            await _productRepository.AddProductAsync(newProduct);
        }

        public async Task DeleteProductAsync(int productId, string ownerId)
        {
            var user = await _userService.GetUserOrThrowAsyncId(ownerId);

            var product = await GetProductOrThrowIdAsync(productId);

            if (product.OwnerId != user.Id)
            {
                throw new UnauthorizedAccessException("You do not have permission to delete this product");
            }
            await _productRepository.DeleteProductAsync(product);
        }

        public async Task<ProductResponse> GetProductByIdAsync(int productId)
        {
            var product = await GetProductOrThrowIdAsync(productId);
            return new ProductResponse
            {
                Name = product.Name,
                Description = product.Description,
                Category = product.Category,
                Price = product.Price,
                Quantity = product.Quantity,
                ImageData = product.ImageData,
                ImageMimeType = product.ImageMimeType,
                ShopId = product.ShopId
            };
        }

        public async Task<Product> GetProductOrThrowIdAsync(int productId)
        {
            var product = await _productRepository.GetProductAsync(productId);
            if (product == null)
            {
                throw new ArgumentException("Product not found", nameof(productId));
            }
            return product;
        }

        public Task<List<ProductResponse>> GetProductsAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<List<ProductResponse>> GetProductsByNameAsync(string productName)
        {
            var products = await _productRepository.GetProductsByNameAsync(productName);

            var productsResponse = products.Select(p => new ProductResponse
            {
                Name = p.Name,
                Description = p.Description,
                Category = p.Category,
                Price = p.Price,
                Quantity = p.Quantity,
                ImageData = p.ImageData,
                ImageMimeType = p.ImageMimeType,
                ShopId = p.ShopId
            }).ToList();

            return productsResponse;
        }

        public async Task<List<ProductResponse>> GetProductsByShopAsync(int shopId)
        {
            var shop = await _shopService.GetShopOrThrowAsync(shopId);

            var products = await _productRepository.GetProductsByShopAsync(shop);
            return products.Select(p => new ProductResponse
            {
                Name = p.Name,
                Description = p.Description,
                Category = p.Category,
                Price = p.Price,
                Quantity = p.Quantity,
                ImageData = p.ImageData,
                ImageMimeType = p.ImageMimeType,
                ShopId = p.ShopId
            }).ToList();
        }

        public async Task<List<ProductResponse>> SearchProductsByNameAsync(string name)
        {
            return await GetProductsByNameAsync(name);
        }

        public async Task UpdateProductAsync(UpdateProductRequest request, string ownerId)
        {
            var user = await _userService.GetUserOrThrowAsyncId(ownerId);

            var product = await GetProductOrThrowIdAsync(request.ProductId);
            if (product.OwnerId != user.Id)
            {
                throw new UnauthorizedAccessException("You do not have permission to update this product");
            }
            product.Name = request.Name;
            product.Description = request.Description;
            product.Category = request.Category;
            product.Price = request.Price;
            product.Quantity = request.Quantity;
            product.ImageData = request.ImageData;
            product.ImageMimeType = request.ImageMimeType;

            await _productRepository.UpdateProductAsync(product);

        }
    }
}
