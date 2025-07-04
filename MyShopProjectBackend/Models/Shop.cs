using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyShopProjectBackend.Models
{
    public class Shop
    {
        [Required]
        public int Id { get; set; } // Ідентифікатор магазину
		[Required]
		public string Name { get; set; } = string.Empty; // Назва магазину
        public string Description { get; set; } = string.Empty; // Опис магазину
        public int OwnerId { get; set; } // Ідентифікатор власника магазину (користувача)
       
       // public User User { get; set; }  // Власник магазину (користувач)

        public ICollection<Product> Products { get; set; } = new List<Product>(); // Колекція продуктів, які продаються в магазині
        public ICollection<ShopOrder> Orders { get; set; } = new List<ShopOrder>(); // Колекція замовлень, які були зроблені в магазині
    }
}
