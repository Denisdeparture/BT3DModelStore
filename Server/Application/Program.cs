using BuisnesLogic.Extensions;
using BuisnesLogic.Service;
using BuisnesLogic.ServicesInterface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using DataBase.AppDbContexts;
using BuisnesLogic.Service.Clients;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using BuisnesLogic.Service.Managers;
using DomainModel.Entity;
namespace ApplicationInfrastructure
{
    public class Program
    {
        
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Configuration.AddPantryStorage("11386f7e-195b-4cfe-9e09-081010a4a279", "PantryStorage");
            builder.Services.AddControllersWithViews();
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
                opt.LogoutPath = "/Account/Logout";
            });
            builder.Services.AddMyValidations();
            builder.Services.AddKafkaClient<User>(builder.Configuration);
            builder.Services.AddJwtManager();
            builder.Services.AddSmtpClient(builder.Configuration);
            var app = builder.Build();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCookiePolicy(new CookiePolicyOptions()
            {
                MinimumSameSitePolicy = SameSiteMode.None
            });
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Main}/{action=Catalog}");

            app.Run();
        }
    }
}
