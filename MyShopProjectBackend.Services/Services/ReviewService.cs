using MyShopProjectBackend.Domain.Entities;
using MyShopProjectBackend.Domain.Request.Review;
using MyShopProjectBackend.Domain.Responses;
using MyShopProjectBackend.Repositories.Interfaces;
using MyShopProjectBackend.Services.Interface;

namespace MyShopProjectBackend.Services.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IProductService _productService;
        private readonly IUserService _userService;

        public ReviewService( IReviewRepository reviewRepository, IProductService productService, IUserService userService)
        {
            _reviewRepository = reviewRepository;
            _productService = productService;
            _userService = userService;
        }
        public async Task AddReviewAsync(AddReviewRequest request, string userId)
        {
            var user = await _userService.GetUserOrThrowAsyncId(userId);

            var product = await _productService.GetProductOrThrowIdAsync(request.ProductId);

            var review = new ProductReview
            {
                ProductId = request.ProductId,
                UserId = user.Id,
                Rating = request.Rating,
                ReviewText = request.Content
            };
            await _reviewRepository.AddReviewAsync(review);

        }

        public async Task DeleteReviewAsync(int productId, string userId)
        {
            var user = await _userService.GetUserOrThrowAsyncId(userId);

            var product = await _productService.GetProductOrThrowIdAsync(productId);

            var review = await _reviewRepository.GetReviewsByUserAndProductAsync(userId, productId);
            if (review == null)
            {
                throw new InvalidOperationException("Review not found");
            }

            await _reviewRepository.DeleteReviewAsync(review);
        }

        public async Task<List<ReviewResponse>> GetReviewsByProductAsync(int productId)
        {
            var product = await _productService.GetProductOrThrowIdAsync(productId);

            var reviews = await _reviewRepository.GetReviewsByProductAsync(product);

            var reviewResponses = reviews.Select(r => new ReviewResponse
            {
                Id = r.Id,
                UserName = r.UserName,
                Rating = r.Rating,
                ReviewText = r.ReviewText,
                CreatedAt = r.CreatedAt
            }).ToList();

            return reviewResponses;
        }

        public async Task UpdateReviewAsync(UpdateReviewRequest request, string userId)
        {
            var user = await _userService.GetUserOrThrowAsyncId(userId);

            var review = await _reviewRepository.GetReviewAsync(new ProductReview { Id = request.ReviewId });
            if (review == null || review.UserId != user.Id)
            {
                throw new InvalidOperationException("Review not found or you do not have permission to update this review");
            }

            review.Rating = request.Rating;
            review.ReviewText = request.Content;
            review.UserId = user.Id;

            await _reviewRepository.UpdateReviewAsync(review);
        }
    }
}
