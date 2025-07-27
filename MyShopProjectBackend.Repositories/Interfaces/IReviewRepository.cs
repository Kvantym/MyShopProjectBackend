using MyShopProjectBackend.Domain.Entities;

namespace MyShopProjectBackend.Repositories.Interfaces
{
    public interface IReviewRepository
    {
        Task AddReviewAsync(ProductReview review);
        Task UpdateReviewAsync(ProductReview review);
        Task DeleteReviewAsync(ProductReview review);
        Task<List<ProductReview>> GetReviewsByProductAsync(Product product);
        Task<List<ProductReview>> GetReviewsByUserAsync(ApplicationUser user);
        Task<ProductReview?> GetReviewAsync(ProductReview review);
        Task<List<ProductReview>> GetAllReviewsAsync();
        Task<ProductReview> GetReviewsByUserAndProductAsync(string userId, int productId);
    }
}
