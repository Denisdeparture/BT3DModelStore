using AppServiceInterfice.Extensions;
using DomainModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WebClient.DbContexts;
using WebServer.AppDbContexts;
namespace ApplicationInfrastructure
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
          //  builder.Services.AddMemoryCache();
            builder.Services.AddAuthentication()
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
            });
            builder.Services.AddDbContext<MainClientDbContext>(opt => opt.UseSqlServer(@"Server=WIN-Q0NS67721NA\SQLEXPRESS;Database=ApplicationSignInManager;Trusted_Connection=True;TrustServerCertificate=True;"));
            builder.Services.AddIdentity<User, IdentityRole>(opt => opt.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<MainClientDbContext>();
            builder.Services.ConfigureApplicationCookie(opt =>
            {
                opt.LoginPath = "/Account/Login";
              //  opt.LogoutPath = "/Account/Logout";
            });
            var app = builder.Build();
           
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Main/Error");
                app.UseHsts();
            }
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
