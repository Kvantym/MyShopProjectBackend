using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyShopProjectBackend.Db;
using MyShopProjectBackend.DTO;
using MyShopProjectBackend.Models;
using MyShopProjectBackend.Servises.Interface;
using MyShopProjectBackend.ViewModels;
using System.Linq;

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
        public async Task<(bool Success, string? ErrorMessage)> AddReviewAsync(CreateReviewModel model)
        {
            var product = await _context.products.FindAsync(model.ProductId);
            if (product == null)
            {
                return (false,"Товар не знайдено");
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

            return (true,null);
        }

        public async Task<(bool Success, string? ErrorMessage)> DeleteReviewAsync(DeleteReviewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId.ToString());
            if (user == null)
            {
                return (false,"Користувача не знайдено");
            }

            var review = await _context.productReviews.FindAsync(model.ReviewId);
            if (review == null)
            {
                return (false, "Відгук не знайдено");
            }

            if (review.UserId != model.UserId)
            {
                return (false, "Ви не маєте права видаляти цей відгук");
            }

            _context.productReviews.Remove(review);
            await _context.SaveChangesAsync();

            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage, List<ReviewDto> Reviews)> GetReviewsByProductAsync(int productId)
        {
            var product = await _context.products.FindAsync(productId);
            if (product == null)
            {
                return (false, "Товар не знайдено", new List<ReviewDto>());
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
                UserName = users.FirstOrDefault(u => u.Id == r.UserId.ToString())?.UserName ?? "Unknown",

                CreatedAt = r.CreatedAt
            }).ToList();

            return (true, null, reviews);
        }


        public async Task<(bool Success, string? ErrorMessage)> UpdateReviewAsync(UpdateReviewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId.ToString());
            if (user == null)
            {
                return (false, "Користувача не знайдено");
            }

            var review = await _context.productReviews.FindAsync(model.ReviewId);
            if (review == null)
            {
                return (false, "Відгук не знайдено");
            }
            if (review.UserId != model.UserId)
            {
                return (false, "Ви не маєте права редагувати цей відгук");
            }

            review.Rating = model.Rating;
            review.ReviewText = model.Content;
            await _context.SaveChangesAsync();

            return (true,null);
        }
    }
}
