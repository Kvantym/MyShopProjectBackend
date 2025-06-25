
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
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
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superSecretKey1234567890!@#$%^&*()_+QWERTY"))
                };
            });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(); // Додайте Swagger для документації API

            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Введіть токен у форматі: Bearer {токен}",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
            });



            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication(); // Додайте аутентифікацію до конвеєра обробки запитів
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
