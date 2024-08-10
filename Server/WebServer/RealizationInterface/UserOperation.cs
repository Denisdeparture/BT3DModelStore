using Infrastructure;
using Microsoft.AspNetCore.Identity;
using System.Collections;
using BuisnesLogic.Model.Roles;
using BuisnesLogic.Model;
using Infrastructure.ModelResult;
using System.Linq.Expressions;
using DomainModel.Entity;
namespace WebServer.RealizationInterface
{
    public partial class UserOperation : IUserOperation, IDeleteUserOperation
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<Program> _logger;
        public UserOperation(UserManager<User> usermanager, RoleManager<IdentityRole> rolemanager, ILogger<Program> logger) 
        { 
            _userManager = usermanager;
            _roleManager = rolemanager;
            _logger = logger;
        }
        public async Task<UserOperationModel> Delete(string id)
        {
            var model = new UserOperationModel()
            {
                IsSuccesed = true,
                Errors = new List<Exception>()
            };
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                var res = await _userManager.DeleteAsync(user!);
                if (res.Errors.ToList().Count != 0)
                {
                    foreach (var error in res.Errors)
                    {
                        model.Errors.Add(new Exception(error.Description));
                    }
                    model.IsSuccesed = false;
                    return model;
                }
            }
            catch(Exception ex)
            {
                model.Errors.Add(ex);
                model.IsSuccesed = false;
                return model;
            }
          
           
            return new UserOperationModel() { IsSuccesed = true };
        }
        public IEnumerable<User> GetAllUsers()
        {
            return _userManager.Users;
        }
        public async Task<UserOperationModel> Register(User user)
        {
            var model = new UserOperationModel()
            {
                IsSuccesed = true,
                Errors = new List<Exception>()
            };
            var resCreate = await CreateUserAsync(user);
            if (!resCreate.IsSuccesed)
            {
                model.IsSuccesed = false;
                foreach(var error in resCreate.Errors!)
                {
                    model.Errors.Add(error);
                }
                return model;
            }
            try
            {
                var task = _userManager.AddToRoleAsync(user, RolesType.User.ToString());
                task.Wait();
                var res = task.Result;
                if (res.Errors.ToList().Count != 0)
                {
                    foreach (var error in res.Errors)
                    {
                        model.Errors.Add(new Exception(error.Description));
                    }
                    model.IsSuccesed = false;
                    return model;
                }
            }
            catch(Exception ex)
            {
                model.Errors.Add(ex);
                model.IsSuccesed = false;
                return model;
            }
            return model;
        }
        public async Task<UserOperationModel> UpdateAsync(string id, User newdatauser)
        {
            var model = new UserOperationModel()
            {
                IsSuccesed = true,
                Errors = new List<Exception>()
            };
            User? nowUser = null; 
            try
            {
                nowUser = await _userManager.FindByIdAsync(id);
                if (nowUser is null)
                {
                    model.Errors.Add(new Exception($"updating was incorrect because user was null"));
                    model.IsSuccesed = false;
                    return model;
                }
            }
            catch(Exception ex)
            {
                model.Errors.Add(ex);
                model.IsSuccesed = false;
                return model;
            }
            try
            {
                nowUser = newdatauser;
                var result = await _userManager.UpdateAsync(nowUser);
                if (result.Errors.ToList().Count != 0)
                {
                    foreach (var error in result.Errors)
                    {
                        model.Errors.Add(new Exception(error.Description));
                    }
                    model.IsSuccesed = false;
                    return model;
                }
                return model;
            }
            catch (Exception ex)
            {
                return new UserOperationModel { IsSuccesed = false, Errors = new List<Exception>() { ex } };
            }
        
        }
       
        private async Task<UserOperationModel> CreateUserAsync(User user)
        {
            var model = new UserOperationModel()
            {
                IsSuccesed = true,
                Errors = new List<Exception>()
            };
            try
            {
                var userExist = await _userManager.FindByEmailAsync(user.Email!);
                if (userExist is not null)
                {
                    model.IsSuccesed = false;
                    model.Errors.Add(new Exception($"{userExist.Email} was not null"));
                    return model;
                }
            }
            catch(Exception ex)
            {
                model.IsSuccesed = false;
                model.Errors.Add(ex);
                return model;
            }
            try
            {
                var res = await _userManager.CreateAsync(user);
                if (res.Errors.ToList().Count != 0)
                {
                    model.IsSuccesed = false;
                    foreach (var error in res.Errors)
                    {
                        model.Errors.Add(new Exception(error.Description));
                    }
                    return model;
                }
                return model;
            }
            catch (Exception ex)
            {
                model.IsSuccesed = false;
                model.Errors.Add(ex);
                return model;
            }
   
        }
    }
}
