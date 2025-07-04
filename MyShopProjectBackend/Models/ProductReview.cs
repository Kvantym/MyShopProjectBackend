using MyShopProjectBackend.DTO;

namespace MyShopProjectBackend.Models
{
    public class ProductReview: ReviewDto
    {
		public int ProductId { get; set; } 
		public Product Product { get; set; }
		public int UserId { get; set; } 

		public ProductReview()
		{
			CreatedAt = DateTime.UtcNow; 
		}
	}
}
