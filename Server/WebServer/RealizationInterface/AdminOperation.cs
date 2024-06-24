using Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace WebServer.RealizationInterface
{
    public class AdminOperation : IRoleOperation
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        
        private readonly ILogger _logger;
        public AdminOperation(RoleManager<IdentityRole> rolemanager, ILogger logger)
        {
            _roleManager = rolemanager;
            _logger = logger;
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

        
    }
}
