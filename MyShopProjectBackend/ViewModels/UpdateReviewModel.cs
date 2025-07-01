namespace MyShopProjectBackend.ViewModels
{
    public class UpdateReviewModel
    {
        public int UserId { get; set; }
        public int ReviewId { get; set; }
        public string Content { get; set; }
        public int Rating { get; set; }
    }
}
