using MyShopProjectBackend.DTO;
using MyShopProjectBackend.ViewModels;

namespace MyShopProjectBackend.Servises.Interface
{
    public interface IReviewServise
    {
        public Task<(bool Success, string? ErrorMessage)> AddReviewAsync(CreateReviewModel model); // Асинхронний метод для додавання відгуку до продукту
        public Task<(bool Success, string? ErrorMessage)> UpdateReviewAsync(UpdateReviewModel model); // Асинхронний метод для оновлення відгуку
        public Task<(bool Success, string? ErrorMessage)> DeleteReviewAsync(DeleteReviewModel model); // Асинхронний метод для видалення відгуку
        public Task<(bool Success, string? ErrorMessage, List<ReviewDto> Reviews)> GetReviewsByProductAsync(int productId); // Асинхронний метод для отримання всіх відгуків по продукту
    }
}
