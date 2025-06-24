using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyShopProjectBackend.Db;
using MyShopProjectBackend.ViewModels;

namespace MyShopProjectBackend.Controllers
{
    public class ReviewController : Controller
    {
        private readonly AppDbConection _context;

        public ReviewController(AppDbConection conection)
        {
            _context = conection;
        }

        // GET: ReviewController
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddReview([FromBody] CreateReviewModel model)
        {
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateReview([FromBody] UpdateReviewModel model)
        {
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteReview(int reviewId)
        {
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetReviewsByProduct(int productId)
        {
            return Ok();
        }

    }
}
