
using BuisnesLogic.Service.Clients;
using BuisnesLogic.ServicesInterface.ClientsInterfaces.MessageBrokers;
using DomainModel;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace WebServer.BackgroundServices
{
    public class RegistrationService : BackgroundService
    {
        private readonly IConsumeClient<User> _messageBroker;
        private readonly ILogger<Program> _logger;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        //private readonly IServiceProvider _serviceProvider;
        private readonly UnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        public readonly CancellationTokenSource _tokenSource;
        public RegistrationService(ILogger<Program> logger, IServiceProvider serviceProvider,IMessageBrokerClient<User> client,IConfiguration configuration)
        {
            _messageBroker = client;
            _tokenSource = new CancellationTokenSource();
            var scopeUserManager = serviceProvider.CreateScope();
            _userManager = scopeUserManager.ServiceProvider.GetRequiredService<UserManager<User>>();
            var scopeRoleManager = serviceProvider.CreateScope();
            _roleManager = scopeRoleManager.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>(); 
            _unitOfWork = new UnitOfWork(_userManager, _roleManager, logger);
            _configuration = configuration;
            _logger = logger;
            //_serviceProvider = serviceProvider;
            var users = _messageBroker.Models;
            users.CollectionChanged += Users_Register;
        }
        private void Users_Register(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if(e.Action == NotifyCollectionChangedAction.Add)
            {
                Register(e.NewItems!.Cast<User>());
            }
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var task = _unitOfWork.adminOperations.CreateRole("Test");
            task.Wait();
            // Test Functional ^
            var factoryTask = new TaskFactory(stoppingToken);
            var tasks = new[] 
            {
              factoryTask.StartNew(() => _messageBroker.Consume(60, _tokenSource)),
              factoryTask.StartNew(() => _messageBroker.Consume(60, _tokenSource)) 
            };
            return Task.CompletedTask;
        }
        private async void Register(IEnumerable<User> users)
        {
            foreach (var user in users)
            {
                var userexist = await _userManager.FindByEmailAsync(user.Email!);
                if (userexist is null)
                {
                    var res = await _unitOfWork.userOperations.Register(user, _configuration["Roles:User"]!);
                    if (!res)
                    {
                        const string message = "user is not added";
                        throw new Exception(DateTime.UtcNow + Environment.NewLine + this.ToString() + Environment.NewLine + message);
                    }
                }
            }

        }
    }
}
