using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyShopProjectBackend.Db;
using MyShopProjectBackend.ViewModels;
using System.Security.Claims;

namespace MyShopProjectBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewController : Controller
    {
        private readonly AppDbConection _context;

        public ReviewController(AppDbConection conection)
        {
            _context = conection;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
        [Authorize]
        [HttpPost("AddReview")]
        public async Task<IActionResult> AddReview([FromBody] CreateReviewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr))
            {
                return Unauthorized("Користувач не авторизований");
            }    

            int userId = int.Parse(userIdStr);

            var product = await _context.products.FindAsync(model.ProductId);
            if (product == null)
            {
                return BadRequest("Товар не знайдено");
            }

            var review = new Models.ProductReview
            {
                ProductId = model.ProductId,
                UserId = userId,
                Rating = model.Rating,
                ReviewText = model.Content,
            };

            _context.productReviews.Add(review);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Відгук успішно відредаговано" });

        }
        [Authorize]
        [HttpPost("UpdateReview")]
        public async Task<IActionResult> UpdateReview([FromBody] UpdateReviewModel model)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr))
            {
                return Unauthorized("Користувач не авторизований");
            }

            int userId = int.Parse(userIdStr);

            var user = await _context.users.FindAsync(userId);
            if (user == null)
            {
                return NotFound("Користувача не знайдено");
            }

            var review = await _context.productReviews.FindAsync(model.ReviewId);
            if (review == null)
            {
                return NotFound("Відгук не знайдено");
            }
             if (review.UserId != userId)
            {
                return Forbid("Ви не маєте права редагувати цей відгук");
            }

             review.Rating = model.Rating;
             review.ReviewText = model.Content;
             await _context.SaveChangesAsync();

            return Ok(new { message = "Відгук успішно відредаговано" });

        }
        [Authorize]
        [HttpPost("DeleteReview")]
        public async Task<IActionResult> DeleteReview(int reviewId)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr))
            {
                return Unauthorized("Користувач не авторизований");
            }

            int userId = int.Parse(userIdStr); 

            var user = await _context.users.FindAsync(userId);
            if (user == null)
            {
                return NotFound("Користувача не знайдено");
            }

            var review = await _context.productReviews.FindAsync(reviewId);
            if (review == null)
            {
                return NotFound("Відгук не знайдено");
            }

            if(review.UserId != userId)
            {
                return Forbid("Ви не маєте права видаляти цей відгук");
            }

            _context.productReviews.Remove(review);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Відгук успішно відредаговано" });

        }

        [HttpGet("GetReviewsByProduct")]
        public async Task<IActionResult> GetReviewsByProduct(int productId)
        {
            var product = await _context.products.FindAsync(productId);
            if (product == null)
            {
                return NotFound("Товар не знайдено");
            }

            var reviews = await _context.productReviews.Where(r=> r.ProductId == productId).Select(r=> new
            {
                r.Id,
                r.Rating,
                r.ReviewText,
                UserName = _context.users.Where(u => u.Id == r.UserId).Select(u => u.UserName).FirstOrDefault(),
                CreatedAt = r.CreatedAt
            }).ToListAsync();
            return Ok(reviews);
        }

    }
}
