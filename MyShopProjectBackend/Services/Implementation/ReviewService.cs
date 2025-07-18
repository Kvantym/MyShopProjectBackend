
using Microsoft.EntityFrameworkCore;
using MyShopProjectBackend.Db;
using MyShopProjectBackend.DTO;
using MyShopProjectBackend.Entities;
using MyShopProjectBackend.Exceptions;
using MyShopProjectBackend.Models.Review;
using MyShopProjectBackend.Servises.Interface;
using NLog;

namespace MyShopProjectBackend.Services.Implementation
{
    public class ReviewService : IReviewService
    {
        private readonly AppDbConection _context;
        private readonly NLog.ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly IUserService _userServise;
        private readonly IProductService _productServises;

        public ReviewService(AppDbConection context, IUserService userServise, IProductService productServises)
        {
            _context = context;
            _userServise = userServise;
            _productServises = productServises;
        }

        public async Task AddReviewAsync(CreateReviewModel model, string userId)
        {
            _logger.Info($"{nameof(AddReviewAsync)}: Виклик методу");
            var user = await _userServise.GetUserOrThrowAsyncId(userId);

            var product = await _productServises.GetProductOrThrowIdAsync(model.ProductId);

            var review = new ProductReview
            {
                ProductId = model.ProductId,
                UserName = user.UserName,
                Rating = model.Rating,
                ReviewText = model.Content,
            };

            _context.productReviews.Add(review);
            await _context.SaveChangesAsync();

            _logger.Info($"{nameof(AddReviewAsync)}: Відгук додано для товару {product.Name}, {nameof(AddReviewAsync)}: Успішно виконано");
        }

        public async Task DeleteReviewAsync(int reviewId, string userId)
        {
            _logger.Info($"{nameof(DeleteReviewAsync)}: Виклик методу");
            var user = await _userServise.GetUserOrThrowAsyncId(userId);

            var review = await GetReviewOrThrowAsync(reviewId);

            if (review.UserId != user.Id)
            {
                _logger.Warn($"{nameof(DeleteReviewAsync)}: Користувач {user.UserName} не має права видаляти відгук з ID {review.Id}");
                throw new BadRequestException("Ви не маєте права видаляти цей відгук");
            }

            _context.productReviews.Remove(review);
            await _context.SaveChangesAsync();

            _logger.Info($"{nameof(DeleteReviewAsync)}: Відгук з ID {review.Id} видалено, {nameof(DeleteReviewAsync)}: Успішно виконано");
        }

        public async Task<List<ReviewDto>> GetReviewsByProductAsync(int productId)
        {
            _logger.Info($"{nameof(GetReviewsByProductAsync)}: Виклик методу");

            var product = await _productServises.GetProductOrThrowIdAsync(productId);

            var reviewsEntities = await _context.productReviews
                .Where(r => r.ProductId == productId)
                .ToListAsync();

            if (reviewsEntities.Count == 0)
            {
                _logger.Warn($"{nameof(GetReviewsByProductAsync)}: Відгуків для товару {product.Name} не знайдено");
            }

            var userIds = reviewsEntities.Select(r => r.UserId).Distinct().ToList();
            var users = await _userServise.GetUsersByIdsAsync(userIds);

            var reviews = reviewsEntities.Select(r => new ReviewDto
            {
                Id = r.Id,
                Rating = r.Rating,
                ReviewText = r.ReviewText,
                UserName = users.FirstOrDefault(u => u.Id == r.UserId)?.UserName,
                CreatedAt = r.CreatedAt
            }).ToList();

            _logger.Info($"{nameof(GetReviewsByProductAsync)}: Отримано {reviews.Count} відгуків для товару {product.Name}, {nameof(GetReviewsByProductAsync)}: Успішно виконано");
            return reviews;
        }

        public async Task UpdateReviewAsync(UpdateReviewModel model, string userId)
        {
            _logger.Info($"{nameof(UpdateReviewAsync)}: Виклик методу");
            var user = await _userServise.GetUserOrThrowAsyncId(userId);

            var review = await GetReviewOrThrowAsync(model.ReviewId);

            if (review.UserId != user.Id)
            {
                _logger.Warn($"{nameof(UpdateReviewAsync)}: Користувач {user.UserName} не має права редагувати відгук з ID {review.Id}");
                throw new BadRequestException("Ви не маєте права редагувати цей відгук");
            }

            review.Rating = model.Rating;
            review.ReviewText = model.Content;
            await _context.SaveChangesAsync();

            _logger.Info($"{nameof(UpdateReviewAsync)}: Відгук з ID {review.Id} оновлено, {nameof(UpdateReviewAsync)}: Успішно виконано");
        }
        private async Task<ProductReview> GetReviewOrThrowAsync(int reviewId)
        {
            _logger.Info($"{nameof(GetReviewOrThrowAsync)}: Виклик методу");
            var review = await _context.productReviews.FindAsync(reviewId);
            if (review == null)
            {
                _logger.Warn($"{nameof(GetReviewOrThrowAsync)}: Відгук з ID {reviewId} не знайдено");
                throw new NotFoundException("Відгук не знайдено");
            }
            return review;
        }
    }
}
