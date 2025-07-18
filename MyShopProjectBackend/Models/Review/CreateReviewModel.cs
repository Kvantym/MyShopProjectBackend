namespace MyShopProjectBackend.Models.Review
{
    public class CreateReviewModel
    {
        public int ProductId { get; set; }
 
        public string Content { get; set; }
        public int Rating { get; set; }
    }
}
