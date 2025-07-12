using MyShopProjectBackend.DTO;
using MyShopProjectBackend.Models.Review;

namespace MyShopProjectBackend.Servises.Interface
{
    public interface IReviewServise
    {
        public Task AddReviewAsync(CreateReviewModel model, string userId); 
        public Task UpdateReviewAsync(UpdateReviewModel model, string userId); 
        public Task DeleteReviewAsync(int productId, string userId); 
        public Task<List<ReviewDto>> GetReviewsByProductAsync(int productId); 
    }
}
