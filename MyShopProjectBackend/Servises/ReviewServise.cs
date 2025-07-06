using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyShopProjectBackend.Db;
using MyShopProjectBackend.DTO;
using MyShopProjectBackend.Exceptions;
using MyShopProjectBackend.Models;
using MyShopProjectBackend.Servises.Interface;
using MyShopProjectBackend.ViewModels.Create;
using MyShopProjectBackend.ViewModels.Delete;
using MyShopProjectBackend.ViewModels.Update;

namespace MyShopProjectBackend.Servises
{
    public class ReviewServise : IReviewServise
    {
        private readonly AppDbConection _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReviewServise(AppDbConection context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task AddReviewAsync(CreateReviewModel model)
        {
            var product = await _context.products.FindAsync(model.ProductId);
            if (product == null)
            {
                throw new NotFoundException("Товар не знайдено");
            }

            var review = new Models.ProductReview
            {
                ProductId = model.ProductId,
                UserId = model.UserId,
                Rating = model.Rating,
                ReviewText = model.Content,
            };

            _context.productReviews.Add(review);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteReviewAsync(DeleteReviewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId.ToString());
            if (user == null)
            {
                throw new NotFoundException("Користувача не знайдено");
            }

            var review = await _context.productReviews.FindAsync(model.ReviewId);
            if (review == null)
            {
                throw new NotFoundException("Відгук не знайдено");
            }

            if (review.UserId != model.UserId)
            {
                throw new BadRequestException("Ви не маєте права видаляти цей відгук");
            }

            _context.productReviews.Remove(review);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ReviewDto>> GetReviewsByProductAsync(int productId)
        {
            var product = await _context.products.FindAsync(productId);
            if (product == null)
            {
                throw new NotFoundException("Товар не знайдено");
            }

            var reviewsEntities = await _context.productReviews
                .Where(r => r.ProductId == productId)
                .ToListAsync();

            var userIds = reviewsEntities.Select(r => r.UserId.ToString()).Distinct().ToList();

            var users = await _userManager.Users
                .Where(u => userIds.Contains(u.Id))
                .ToListAsync();


            var reviews = reviewsEntities.Select(r => new ReviewDto
            {
                Id = r.Id,
                Rating = r.Rating,
                ReviewText = r.ReviewText,
                UserName = users.FirstOrDefault(u => u.Id == r.UserId.ToString())?.UserName,

                CreatedAt = r.CreatedAt
            }).ToList();
            return reviews;
        }


        public async Task UpdateReviewAsync(UpdateReviewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId.ToString());
            if (user == null)
            {
                throw new NotFoundException("Користувача не знайдено");
            }

            var review = await _context.productReviews.FindAsync(model.ReviewId);
            if (review == null)
            {
                throw new NotFoundException("Відгук не знайдено");
            }

            if (review.UserId != model.UserId)
            {
                throw new BadRequestException("Ви не маєте права редагувати цей відгук");
            }

            review.Rating = model.Rating;
            review.ReviewText = model.Content;
            await _context.SaveChangesAsync();
        }
    }
}
