namespace MyShopProjectBackend.Domain.Entities
{
    public class ProductReview
    {

        public int Id { get; set; }
        public int Rating { get; set; }
        public string ReviewText { get; set; } = string.Empty;
        public ApplicationUser User { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int ProductId { get; set; }
        public Product Product { get; set; }
        public string UserId { get; set; }

    }
}
