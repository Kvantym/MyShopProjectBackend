namespace MyShopProjectBackend.Domain.Request.Cart
{
    public class UpdateCartRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
