using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyShopProjectBackend.Db;
using MyShopProjectBackend.DTO;
using MyShopProjectBackend.Entities;
using MyShopProjectBackend.Exceptions;
using MyShopProjectBackend.Models.Review;
using MyShopProjectBackend.Servises.Interface;
using NLog;

namespace MyShopProjectBackend.Servises
{
    public class ReviewServise : IReviewServise
    {
        private readonly AppDbConection _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly NLog.ILogger _logger = LogManager.GetCurrentClassLogger();

        public ReviewServise(AppDbConection context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task AddReviewAsync(CreateReviewModel model, string userId)
        {
            _logger.Info($"{nameof(AddReviewAsync)}: Виклик методу");
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) 
            {
                _logger.Warn($"{nameof(AddReviewAsync)}: Користувача з ID {userId} не знайдено");
                throw new NotFoundException("Користувача не знайдено");
            } 

            var product = await _context.products.FindAsync(model.ProductId);
            if (product == null)
            {
                _logger.Warn($"{nameof(AddReviewAsync)}: Товар з ID {model.ProductId} не знайдено");
                throw new NotFoundException("Товар не знайдено");
            }

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

        public async Task DeleteReviewAsync(int productId, string userId)
        {
            _logger.Info($"{nameof(DeleteReviewAsync)}: Виклик методу");
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.Warn($"{nameof(DeleteReviewAsync)}: Користувача з ID {userId} не знайдено");
                throw new NotFoundException("Користувача не знайдено");
            }

            var review = await _context.productReviews.FindAsync(productId);
            if (review == null)
            {
                _logger.Warn($"{nameof(DeleteReviewAsync)}: Відгук з ID {productId} не знайдено");
                throw new NotFoundException("Відгук не знайдено");
            }

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
            var product = await _context.products.FindAsync(productId);
            if (product == null)
            {
                _logger.Warn($"{nameof(GetReviewsByProductAsync)}: Товар з ID {productId} не знайдено");
                throw new NotFoundException("Товар не знайдено");
            }

            var reviewsEntities = await _context.productReviews
                .Where(r => r.ProductId == productId)
                .ToListAsync();

            if (reviewsEntities.Count == 0)
            {
                _logger.Warn($"{nameof(GetReviewsByProductAsync)}: Відгуків для товару {product.Name} не знайдено");
            }

            var userIds = reviewsEntities.Select(r => r.UserId).Distinct().ToList();
            var users = await _userManager.Users
                .Where(u => userIds.Contains(u.Id))
                .ToListAsync();

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
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.Warn($"{nameof(UpdateReviewAsync)}: Користувача з ID {userId} не знайдено");
                throw new NotFoundException("Користувача не знайдено");
            }

            var review = await _context.productReviews.FindAsync(model.ReviewId);
            if (review == null)
            {
                _logger.Warn($"{nameof(DeleteReviewAsync)} : Відгук з ID {model.ReviewId} не знайдено");
                throw new NotFoundException("Відгук не знайдено");
            }

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
    }
}
