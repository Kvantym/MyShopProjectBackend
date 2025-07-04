namespace MyShopProjectBackend.ViewModels
{
    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
       
        public LoginModel(string username, string password, string email)
        {
            Username = username;
            Password = password;
            Email = email;
        }
       
    }
}
