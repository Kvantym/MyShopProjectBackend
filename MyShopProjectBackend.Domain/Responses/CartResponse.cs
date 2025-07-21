namespace MyShopProjectBackend.Domain.Responses
{
    public class CartResponse
    {
        public int Id { get; set; }
        public  string UserId { get; set; }
        public List<CartItemResponse> Items { get; set; } = new List<CartItemResponse>();
    }
}
