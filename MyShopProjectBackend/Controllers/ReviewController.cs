using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyShopProjectBackend.Extensions;
using MyShopProjectBackend.Models.Review;
using MyShopProjectBackend.Servises.Interface;

namespace MyShopProjectBackend.Controllers
{
    [ApiController]
    [Route("api/review")]
    public class ReviewController : Controller
    {
        private readonly IReviewService _reviewServise;

        public ReviewController(IReviewService reviewServise)
        {
            _reviewServise = reviewServise;
        }

        [HttpGet("status")]
        public IActionResult Get() => Ok("API працює");//готово

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddReview([FromBody] CreateReviewModel model)//готово
        {
            await _reviewServise.AddReviewAsync(model, User.GetUserId());
            return Ok(new { message = "Відгук успішно додано" });
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> UpdateReview([FromBody] UpdateReviewModel model)//готово
        {
            await _reviewServise.UpdateReviewAsync(model, User.GetUserId());
            return Ok(new { message = "Відгук успішно відредаговано" });
        }
        [Authorize]
        [HttpDelete("{reviewId}")]
        public async Task<IActionResult> DeleteReview(int reviewId)//готово
        {
            await _reviewServise.DeleteReviewAsync(reviewId, User.GetUserId());
            return Ok(new { message = "Відгук успішно видалено" });
        }

        [HttpGet("reviews")]
        public async Task<IActionResult> GetReviewsByProduct(int productId)//готово
        {
            var result = await _reviewServise.GetReviewsByProductAsync(productId);
            return Ok(result);
        }

    }
}
