namespace MyShopProjectBackend.Models
{
    public class User
    {
        public int Id { get; set; } // Ід користувача
        public string UserName { get; set; } = string.Empty; // Ім'я користувача
        public string Password { get; set; } = string.Empty; // Пароль користувача
        public string Email { get; set; } = string.Empty;// Електронна пошта користувача

        public string Role { get; set; } // Роль користувача

        public ICollection<Shop> shops { get; set; } = new List<Shop>();// Колекція магазинів, які належать користувачу
        public ICollection<Order> orders { get; set; } = new List<Order>();// Колекція замовлень, які зробив користувач

        public ICollection<FavoritProduct> favoritProducts { get; set; } = new List<FavoritProduct>(); // Колекція улюблених продуктів користувача

    }
}
