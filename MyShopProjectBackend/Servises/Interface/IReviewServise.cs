using MyShopProjectBackend.DTO;
using MyShopProjectBackend.ViewModels.Create;
using MyShopProjectBackend.ViewModels.Delete;
using MyShopProjectBackend.ViewModels.Update;

namespace MyShopProjectBackend.Servises.Interface
{
    public interface IReviewServise
    {
        public Task AddReviewAsync(CreateReviewModel model); 
        public Task UpdateReviewAsync(UpdateReviewModel model); 
        public Task DeleteReviewAsync(DeleteReviewModel model); 
        public Task<List<ReviewDto>> GetReviewsByProductAsync(int productId); 
    }
}
