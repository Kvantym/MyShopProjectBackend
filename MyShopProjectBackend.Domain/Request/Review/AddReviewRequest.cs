namespace MyShopProjectBackend.Domain.Request.Review
{
    public class AddReviewRequest
    {
        public int ProductId { get; set; }
        public string Content { get; set; }
        public int Rating { get; set; }
    }
}
