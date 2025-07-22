using Microsoft.EntityFrameworkCore;
using MyShopProjectBackend.Domain.Entities;
using MyShopProjectBackend.Infrastructure;
using MyShopProjectBackend.Repositories.Interfaces;

namespace MyShopProjectBackend.Repositories.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly AppDbConect _context;
        public ReviewRepository(AppDbConect context)
        {
            _context = context;
        }
        public async Task AddReviewAsync(ProductReview review)
        {
           _context.ProductReviews.Add(review);
           await _context.SaveChangesAsync();
        }

        public async Task DeleteReviewAsync(ProductReview review)
        {
           _context.ProductReviews.Remove(review);
           await _context.SaveChangesAsync();
        }

        public async Task<List<ProductReview>> GetAllReviewsAsync()
        {
           return await _context.ProductReviews.Include(r => r.Product).Include(r=>r.User).ToListAsync();
        }

        public async Task<ProductReview?> GetReviewAsync(ProductReview review)
        {
            return await _context.ProductReviews.Include(r => r.Product).Include(r=>r.User).FirstOrDefaultAsync(r => r.Id == review.Id);
        }

        public async Task<List<ProductReview>> GetReviewsByProductAsync(Product product)
        {
            return await _context.ProductReviews.Where(r => r.Product.Id == product.Id).Include(r => r.User).ToListAsync();
        }

        public async Task<List<ProductReview>> GetReviewsByUserAsync(ApplicationUser user)
        {
          return await _context.ProductReviews.Where(r=> r.User.Id == user.Id).Include(r=> r.Product).ToListAsync();
        }

        public async Task UpdateReviewAsync(ProductReview review)
        {
            _context.ProductReviews.Update(review);
            await _context.SaveChangesAsync();
        }
    }
}
