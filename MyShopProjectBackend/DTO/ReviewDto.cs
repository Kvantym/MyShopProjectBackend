namespace MyShopProjectBackend.DTO
{
    public class ReviewDto
    {
        public int Id { get; set; }
        public int Rating { get; set; }
        public string ReviewText { get; set; } = string.Empty;
        public string UserName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
