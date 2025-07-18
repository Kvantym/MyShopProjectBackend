namespace MyShopProjectBackend.DTO
{
    public class CartDto
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public List<CartItemDto> Items { get; set; } = new List<CartItemDto>();
    }
}
