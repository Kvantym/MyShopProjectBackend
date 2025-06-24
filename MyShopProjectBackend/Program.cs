
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyShopProjectBackend.Db;
using System.Text;

namespace MyShopProjectBackend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            builder.Services.AddDbContext<AppDbConection>(optionsAction => optionsAction.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
            // Зареєструйте AppDbConection як сервіс
            builder.Services.AddAuthentication(options => 
            { 
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; // Стандартна схема аутентифікації
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; // Схема для виклику викликів аутентифікації
            })

            .AddJwtBearer(options => 
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = "MyShopProjectBackend",
                    ValidAudience = "MyShopProjectFron",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superSecretKey123456"))
                };
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication(); // Додайте аутентифікацію до конвеєра обробки запитів
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
