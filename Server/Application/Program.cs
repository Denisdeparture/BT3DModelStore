using BuisnesLogic.Extensions;
using BuisnesLogic.Service;
using BuisnesLogic.ServicesInterface;
using DomainModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using DataBase.AppDbContexts;
using BuisnesLogic.Service.Clients;
using Microsoft.AspNetCore.Authentication.JwtBearer;
namespace ApplicationInfrastructure
{
    public class Program
    {
        
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Configuration.AddJsonFile("appsettings.json").AddJsonFile("endpointsconfigure.json");
            builder.Services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddYandex(opt =>
            {
                opt.CallbackPath = builder.Configuration["YandexOAuth2.0:CallbackPath"];
                opt.ClientId = builder.Configuration["YandexOAuth2.0:ClientId"]!;
                opt.ClientSecret = builder.Configuration["YandexOAuth2.0:ClientSecret"]!;
            })
            .AddGoogle(opt =>
            {
                opt.CallbackPath = builder.Configuration["GoogleOAuth2.0:CallbackPath"];
                opt.ClientId = builder.Configuration["GoogleOAuth2.0:ClientId"]!;
                opt.ClientSecret = builder.Configuration["GoogleOAuth2.0:ClientSecret"]!;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                    ValidAudience = builder.Configuration["JwtSettings:Audience"],
                    IssuerSigningKey = AuthExtensions.GetSymmetricSecurityKey(builder.Configuration["JwtSettings:SecurityKey"]!),
                    ValidateIssuerSigningKey = true,
                };
            }).AddBearerToken(IdentityConstants.BearerScheme);
            builder.Services.AddDbContext<MainDbContext>(opt => opt.UseNpgsql(builder.Configuration["ConnectionStrings:DatabaseConnect"]));
            builder.Services.AddIdentity<User, IdentityRole>(opt => opt.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<MainDbContext>();
            builder.Services.ConfigureApplicationCookie(opt =>
            {
                opt.LoginPath = "/Account/Login";
            });
            builder.Services.AddMyValidations();
            using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<KafkaClient<User>> logger = factory.CreateLogger<KafkaClient<User>>();
            builder.Services.AddKafkaClient<User>(builder.Configuration, logger);
            builder.Services.AddJwtManager();
            var app = builder.Build();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Main}/{action=Katalog}");

            app.Run();
        }
    }
}
