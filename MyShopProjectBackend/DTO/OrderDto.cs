namespace MyShopProjectBackend.DTO
{
    public class OrderDto
    {
        public int OrderId { get; set; }
        public string Status { get; set; }
        public List<OrderItemDto> Items { get; set; }
    }
}
