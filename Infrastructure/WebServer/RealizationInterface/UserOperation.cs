using DomainInfrastructure;
using DomainModel;
using Microsoft.AspNetCore.Identity;
using System.Collections;

namespace WebServer.RealizationInterface
{
    public partial class UserOperation : IUserOperation, IDeleteUserOperation
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger _logger;
        public UserOperation(UserManager<User> usermanager, RoleManager<IdentityRole> rolemanager, ILogger logger) 
        { 
            _userManager = usermanager;
            _roleManager = rolemanager;
            _logger = logger;
        }
        public async Task<bool> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            var res = await _userManager.DeleteAsync(user!);
            _logger.LogError(string.Join(Environment.NewLine, res.Errors.Select(obj => obj.Description + Environment.NewLine + obj.Code).ToArray()));
            return res.Succeeded;
        }
        public IEnumerable<User> GetAllUsers()
        {
            return _userManager.Users;
        }
        public async Task<bool> Register(User user,string role)
        {
             var resCreate = await CreateUserAsync(user);
             if (!resCreate)
             {
                _logger.LogError("An error occurred during creation");
                return false;
             }
             if ((await _userManager.GetRolesAsync(user)).Where(r => r == role).SingleOrDefault() != null) 
             {
                _logger.LogError("This user have this role");
                return false;
             }
            var res = await _userManager.AddToRoleAsync(user, (_roleManager.Roles.Where(r => r.Name == role).SingleOrDefault() ?? new IdentityRole() { Name = "Test"}).Name! );
            _logger.LogError(string.Join(Environment.NewLine, res.Errors.Select(obj => obj.Description + Environment.NewLine + obj.Code).ToArray()));
            return res.Succeeded;
        }
        public async Task<bool> Update(string id, User newdatauser)
        {
            var nowUser = await _userManager.FindByIdAsync(id.ToString());
            if(nowUser == null) return false;
            for(int i = 0; i < nowUser.GetType().GetProperties().Length; i++)
            {
                try
                {
                    nowUser.GetType().GetProperties()[i].SetValue(nowUser, newdatauser.GetType().GetProperties()[i]);
                }
                catch(Exception ex)
                {
                    _logger.LogError(ex.Message + ex.StackTrace + ex.Source);
                    return false;
                }
            }
            var res = await _userManager.UpdateAsync(nowUser);
            _logger.LogError(string.Join(Environment.NewLine, res.Errors.Select(obj => obj.Description + Environment.NewLine + obj.Code).ToArray()));
            return res.Succeeded;
        }
        private async Task<bool> CreateUserAsync(User user)
        {
            var correction = await _userManager.FindByEmailAsync(user.Email!);
            if (correction != null)
            {
                _logger.LogWarning(correction.Id + "is using");
                return false;
            }
            var res = await _userManager.CreateAsync(user);
            _logger.LogError(string.Join(Environment.NewLine, res.Errors.Select(obj => obj.Description + Environment.NewLine + obj.Code).ToArray()));
            return res.Succeeded;
        }
    }
    public partial class UserOperation : IEnumerable<User>
    {
        public IEnumerator<User> GetEnumerator()
        {
           foreach (var user in _userManager.Users)
           {
                yield return user; 
           }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var user in _userManager.Users)
            {
                yield return user;
            }
        }
    }
}
