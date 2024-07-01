using BuisnesLogic.Extensions;
using BuisnesLogic.Service;
using BuisnesLogic.ServicesInterface;
using DomainModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using DataBase.AppDbContexts;
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
            builder.Services.AddDbContext<MainDbContext>(opt => opt.UseSqlServer(builder.Configuration["DatabaseConnect"]));
            builder.Services.AddIdentity<User, IdentityRole>(opt => opt.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<MainDbContext>();
            builder.Services.ConfigureApplicationCookie(opt =>
            {
                opt.LoginPath = "/Account/Login";
            });
            builder.Services.AddScoped<IYandexClient, YandexCloudClient>(arg => new YandexCloudClient(builder.Configuration["CloudConnection:ClientId"]!, builder.Configuration["CloudConnection:ClientSecret"]!, "ycbucketbt3dmodel"));
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
