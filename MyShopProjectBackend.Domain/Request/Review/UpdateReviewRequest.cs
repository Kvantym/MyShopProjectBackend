namespace MyShopProjectBackend.Domain.Request.Review
{
    public class UpdateReviewRequest
    {
        public int ReviewId { get; set; }
        public string Content { get; set; }
        public int Rating { get; set; }
    }
}
