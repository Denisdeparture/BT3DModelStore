using DomainModel;
using Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace WebServer.RealizationInterface
{
    public class RolesOperation : IRoleOperation, IUsersRolesOperation
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<User> _userManager;
        
        private readonly ILogger<Program> _logger;
        public RolesOperation(RoleManager<IdentityRole> rolemanager, ILogger<Program> logger, UserManager<User> userManager)
        {
            _roleManager = rolemanager;
            _logger = logger;
            _userManager = userManager;
        }
        public async Task<bool> CreateRole(string rolename)
        {
            if (await _roleManager.FindByNameAsync(rolename) != null)
            {
                _logger.LogDebug("This Role is exist");
                return false;
            }
            var res = await _roleManager.CreateAsync(new IdentityRole() { Name = rolename });
            _logger.LogError(string.Join(Environment.NewLine, res.Errors.Select(obj => obj.Description + Environment.NewLine + obj.Code).ToArray()));
            return res.Succeeded;
        }

        public async Task<bool> DeleteRole(string name)
        {
            var role = await _roleManager.FindByNameAsync(name);
            if (role == null)
            {
                _logger.LogDebug("Role isn`t exist");
                return false;
            }
            var res = await _roleManager.DeleteAsync(role);
            _logger.LogError(string.Join(Environment.NewLine, res.Errors.Select(obj => obj.Description + Environment.NewLine + obj.Code).ToArray()));
            return res.Succeeded;
        }
        public async Task<bool> AddRoleFromUserAsync(string email, string role)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null) return false;
            var userHasThisRole = (await _userManager.GetRolesAsync(user)).Where(r => r.Equals(role)).FirstOrDefault();
            if (string.IsNullOrEmpty(userHasThisRole))
            {
                await _userManager.AddToRoleAsync(user, role);
                return true;
            }
            return false;
        }

    }
}
