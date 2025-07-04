namespace MyShopProjectBackend.ViewModels
{
    public class CreateReviewModel
    {
        public int UserId { get; set; }
        public int ProductId { get; set; }
 
        public string Content { get; set; }
        public int Rating { get; set; }
    }
}
