using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyShopProjectBackend.Extensions;
using MyShopProjectBackend.Servises.Interface;
using MyShopProjectBackend.ViewModels.Add;
using MyShopProjectBackend.ViewModels.Remove;
using MyShopProjectBackend.ViewModels.Update;

namespace MyShopProjectBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {

        private readonly ICartServises _cartServises;


        public CartController(ICartServises cartServises)
        {
            _cartServises = cartServises;
        }
        [Authorize]
        [HttpGet("GetCart")]
        public async Task<IActionResult> GetCart()
        {
            var result = await _cartServises.GetCartAsync(User.GetUserId());
            return Ok(result);
        }

        [Authorize]
        [HttpPost("AddToCart")]
        public async Task<IActionResult> AddToCart(AddToCartModel model)
        {
            model.UserId = User.GetUserId();

            var result = _cartServises.AddToCartAsync(model);
            return Ok(new { message = "Товар успішно додано до кошика" });
        }
        [Authorize]
        [HttpPost("UpdateCart")]
        public async Task<IActionResult> UpdateCart(UpdateCartModel model)
        {
            model.UserId = User.GetUserId();
            var result = _cartServises.UpdateCartAsync(model);

            return Ok(new { message = "Кошик успішно оновлено" });
        }

        [Authorize]
        [HttpPost("RemoveFromCart")]
        public async Task<IActionResult> RemoveFromCart(RemoveCartModel model)
        {
            model.UserId = User.GetUserId();

            var result = _cartServises.RemoveFromCartAsync(model);
            return Ok(new { message = "Товар успішно видалено з кошика" });
        }

        [Authorize]
        [HttpPost("ClearCart")]
        public async Task<IActionResult> ClearCart()
        {
            var result = _cartServises.ClearCartAsync(User.GetUserId());
            return Ok(new { message = "Кошик успішно очищено" });
        }

        [Authorize]
        [HttpPost("Checkout")]
        public async Task<IActionResult> Checkout()
        {
            var result = _cartServises.CheckoutAsync(User.GetUserId());
            return Ok(new { message = "Замовлення успішно оформлено" });
        }
    }
}
