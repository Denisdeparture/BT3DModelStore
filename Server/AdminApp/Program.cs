using BuisnesLogic.Extensions;
using BuisnesLogic.Service.Managers;
using DataBase.AppDbContexts;
using DomainModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
namespace AdminApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Configuration.AddPantryStorage("11386f7e-195b-4cfe-9e09-081010a4a279", "PantryStorage");
            builder.Services.AddDbContext<MainDbContext>(opt =>
            {
                opt.UseNpgsql(builder.Configuration["ConnectionStrings:DatabaseConnect"]);
            });
            builder.Services.AddAuthentication();
            builder.Services.ConfigureApplicationCookie(opt =>
            {
                opt.LoginPath = "/Admin/Login";
                opt.LogoutPath = "/Admin/Logout";
            });
            builder.Services.AddIdentity<User, IdentityRole>(delegate (IdentityOptions opt)
            {
                opt.SignIn.RequireConfirmedAccount = true;
                opt.User.AllowedUserNameCharacters = null;
            }).AddEntityFrameworkStores<MainDbContext>();

            builder.Services.AddControllersWithViews();
            var app = builder.Build();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.UseAuthentication();
            app.MapControllerRoute(
               name: "default",
               pattern: "{controller}/{action}");
            app.Run();
        }
    }
}
