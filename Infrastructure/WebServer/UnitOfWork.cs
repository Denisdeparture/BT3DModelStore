using DomainModel;
using Microsoft.AspNetCore.Identity;
using WebServer.RealizationInterface;

namespace WebServer
{
    public class UnitOfWork 
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger _logger;
        private UserOperation userOperationRep;
        private AdminOperation adminOperationRep;
        public UnitOfWork(UserManager<User> usermanager, RoleManager<IdentityRole> rolemanager, ILogger logger)
        {
            _userManager = usermanager;
            _roleManager = rolemanager;
            _logger = logger;
        }
        public UserOperation userOperations 
        { 
            get 
            {
                if (userOperationRep == null) userOperationRep = new UserOperation(_userManager, _roleManager, _logger);
                return userOperationRep;
            } 
            set { } 
        }
        public AdminOperation adminOperations
        {
            get
            {
                if (adminOperationRep == null) adminOperationRep = new AdminOperation(_roleManager, _logger);
                return adminOperationRep;
            }
            set { }
        }

    }
}
