using BuisnesLogic.Model.Roles;
using Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace BuisnesLogic.Extensions
{
    public static class RolesSeed
    {
        public static IApplicationBuilder UseSeedRoles(this IApplicationBuilder services,IServiceProvider provider)
        {
           var roleManager = provider.CreateScope().ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
           roleManager.CreateAsync(new IdentityRole() { Name = RolesType.User.ToString(), NormalizedName = RolesType.User.ToString().ToUpper() }).Wait();
           roleManager.CreateAsync(new IdentityRole() { Name = RolesType.Admin.ToString(), NormalizedName = RolesType.Admin.ToString().ToUpper() }).Wait();
           return services;
        }
    }
}
