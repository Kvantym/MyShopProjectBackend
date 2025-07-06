using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyShopProjectBackend.Extensions;
using MyShopProjectBackend.Servises.Interface;
using MyShopProjectBackend.ViewModels.Create;
using MyShopProjectBackend.ViewModels.Delete;
using MyShopProjectBackend.ViewModels.Update;

namespace MyShopProjectBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewController : Controller
    {
        private readonly IReviewServise _reviewServise;

        public ReviewController(IReviewServise reviewServise)
        {
            _reviewServise = reviewServise;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return Ok("Review Controller is working");
        }

        [Authorize]
        [HttpPost("AddReview")]
        public async Task<IActionResult> AddReview([FromBody] CreateReviewModel model)
        {
            model.UserId = User.GetUserId();
            var result = _reviewServise.AddReviewAsync(model);

            return Ok(new { message = "Відгук успішно додано" });
        }

        [Authorize]
        [HttpPost("UpdateReview")]
        public async Task<IActionResult> UpdateReview([FromBody] UpdateReviewModel model)
        {
            model.UserId = User.GetUserId();
            var result = _reviewServise.UpdateReviewAsync(model);

            return Ok(new { message = "Відгук успішно відредаговано" });
        }
        [Authorize]
        [HttpPost("DeleteReview")]
        public async Task<IActionResult> DeleteReview(DeleteReviewModel model)
        {
            model.UserId = User.GetUserId();
            var result = _reviewServise.DeleteReviewAsync(model);

            return Ok(new { message = "Відгук успішно видалено" });
        }

        [HttpGet("GetReviewsByProduct")]
        public async Task<IActionResult> GetReviewsByProduct(int productId)
        {
            var result = await _reviewServise.GetReviewsByProductAsync(productId);
            return Ok(result);
        }

    }
}
