namespace MyShopProjectBackend.Domain.Responses
{
    public class OrderResponse
    {
        public int OrderId { get; set; }
        public enum Status;
        public List<OrderItemResponse> Items { get; set; } = new List<OrderItemResponse>();
        public decimal TotalPrice => Items.Sum(item => item.TotalPrice);
    }
}
