namespace MyShopProjectBackend.ViewModels.Create
{
    public class CreateReviewModel
    {
        public string UserId { get; set; }
        public int ProductId { get; set; }
 
        public string Content { get; set; }
        public int Rating { get; set; }
    }
}
