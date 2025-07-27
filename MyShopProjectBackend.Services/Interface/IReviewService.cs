using MyShopProjectBackend.Domain.Request.Review;
using MyShopProjectBackend.Domain.Responses;

namespace MyShopProjectBackend.Services.Interface
{
    public interface IReviewService
    {
        public Task AddReviewAsync(AddReviewRequest request, string userId);
        public Task UpdateReviewAsync(UpdateReviewRequest request, string userId);
        public Task DeleteReviewAsync(int productId, string userId);
        public Task<List<ReviewResponse>> GetReviewsByProductAsync(int productId);
    }
}
