using DomainModel.Entity;
using Infrastructure;
using Infrastructure.ModelResult;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Data;

namespace WebServer.RealizationInterface
{
    public class RolesOperation : IRoleOperation, IUsersRolesOperation
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<User> _userManager;
        public RolesOperation(RoleManager<IdentityRole> rolemanager, UserManager<User> userManager)
        {
            _roleManager = rolemanager;
            _userManager = userManager;
        }
        public async Task<RoleOperationModel> CreateRole(string rolename)
        {
            var model = new RoleOperationModel() { IsSucceced = false, errors = new List<Exception>() };
            if (await _roleManager.FindByNameAsync(rolename) != null)
            {
                model.errors.Add(new Exception("This Role is exist"));
                return model;
            }
            var res = await _roleManager.CreateAsync(new IdentityRole() { Name = rolename });
            if (!res.Succeeded)
            {
                string error = string.Join(Environment.NewLine, res.Errors.Select(obj => obj.Description + Environment.NewLine + obj.Code).ToArray());
                model.errors.Add(new Exception(error));
                return model;
            }
            model.IsSucceced = true;
            return model;
        }

        public async Task<RoleOperationModel> DeleteRole(string name)
        {
            var model = new RoleOperationModel() { IsSucceced = false, errors = new List<Exception>() };
            var role = await _roleManager.FindByNameAsync(name);
            if (role is null)
            {
                string error = "Role isn`t exist";
                model.errors.Add(new Exception(error));
                return model;
            }
            var res = await _roleManager.DeleteAsync(role);
            if (res.Succeeded)
            {
                string error = string.Join(Environment.NewLine, res.Errors.Select(obj => obj.Description + Environment.NewLine + obj.Code).ToArray());
                model.errors.Add(new Exception(error));
                return model;
            }
            model.IsSucceced = true;
            return model;
        }
        public async Task<RoleOperationModel> AddRoleFromUserAsync(string email, string role)
        {
            var user  = await _userManager.FindByEmailAsync(email);
            var model = new RoleOperationModel() { IsSucceced = false, errors = new List<Exception>() };
            if (user is null)
            {
                string error = "user isn`t exist";
                model.errors.Add(new Exception(error));
                return model;
            }
            var task = _userManager.GetRolesAsync(user);
            task.Wait();
            var userHasThisRole = task.Result.Where(r => r.Equals(role)).FirstOrDefault();
            model.IsSucceced = true;
            if (string.IsNullOrEmpty(userHasThisRole))
            {
                await _userManager.AddToRoleAsync(user, role);
                return model;
            }
            return model;
        }

    }
}
