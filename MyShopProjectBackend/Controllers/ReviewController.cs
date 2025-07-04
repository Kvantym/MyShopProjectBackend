using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyShopProjectBackend.Db;
using MyShopProjectBackend.Servises.Interface;
using MyShopProjectBackend.ViewModels;
using System.Security.Claims;

namespace MyShopProjectBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewController : Controller
    {
        private readonly AppDbConection _context;
        private readonly IReviewServise _reviewServise;

        public ReviewController(AppDbConection conection, IReviewServise reviewServise)
        {
            _context = conection;
            _reviewServise = reviewServise;
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
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
            {
                return Unauthorized("Користувач не авторизований");
            }
            model.UserId = userId;

            var result = await _reviewServise.AddReviewAsync(model);
            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok(new { message = "Відгук успішно додано" });

        }
        [Authorize]
        [HttpPost("UpdateReview")]
        public async Task<IActionResult> UpdateReview([FromBody] UpdateReviewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
            {
                return Unauthorized("Користувач не авторизований");
            }
            model.UserId = userId;

             var result = await _reviewServise.UpdateReviewAsync(model);
            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok(new { message = "Відгук успішно відредаговано" });

        }
        [Authorize]
        [HttpPost("DeleteReview")]
        public async Task<IActionResult> DeleteReview(DeleteReviewModel model)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
            {
                return Unauthorized("Користувач не авторизований");
            }
            model.UserId = userId;

           var result = await _reviewServise.DeleteReviewAsync(model);
            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok(new { message = "Відгук успішно видалено" });
        }

        [HttpGet("GetReviewsByProduct")]
        public async Task<IActionResult> GetReviewsByProduct(int productId)
        {
            
            var result = await _reviewServise.GetReviewsByProductAsync(productId);
            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok(result.Reviews);
        }

    }
}
