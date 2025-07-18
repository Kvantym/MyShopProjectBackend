namespace MyShopProjectBackend.DTO
{
    public class OrderDto
    {
        public int OrderId { get; set; }
        public string Status { get; set; }
        public List<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();
        public decimal TotalPrice => Items.Sum(item => item.TotalPrice);
    }
}
