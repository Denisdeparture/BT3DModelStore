using BuisnesLogic.ServicesInterface.ClientsInterfaces.MessageBrokers;
using DomainModel;
using Infrastructure;
using System.Collections.Specialized;
namespace WebServer.BackgroundServices
{
    public class RegistrationService : BackgroundService
    {
        private readonly IConsumeClient<User> _messageBroker;
        private readonly ILogger<Program> _logger;
        private readonly IServiceProvider _serviceProvider;
        public RegistrationService(ILogger<Program> logger,IMessageBrokerClient<User> client, IServiceProvider serviceProvider)
        {
            _messageBroker = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serviceProvider = serviceProvider  ?? throw new ArgumentNullException(nameof(serviceProvider));
            _messageBroker.Models.CollectionChanged += Users_Register;
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
            CancellationTokenSource _tokenSource = new CancellationTokenSource();
            var factoryTask = new TaskFactory(stoppingToken);
            var tasks = new[] 
            {
              factoryTask.StartNew(() => _messageBroker.Consume(20, _tokenSource)),
              factoryTask.StartNew(() => _messageBroker.Consume(20, _tokenSource)) 
            };
            return Task.CompletedTask;
        }
        private async void Register(IEnumerable<User> users)
        {
            var operation = _serviceProvider.CreateScope().ServiceProvider.GetRequiredService<IUserOperation>();
            foreach (var user in users)
            {
                User? userexist = null;
                try
                {
                    userexist = operation.GetAllUsers().Where(us => us.Email!.Equals(user.Email)).SingleOrDefault();
                }
                catch (Exception ex)
                {
                    _logger.LogError(DateTime.UtcNow + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + this.ToString() + " maybe in system was two equals email");
                }
                if (userexist is null)
                {
                    var res = await operation.Register(user);
                    if (!res.IsSuccesed)
                    {
                        _logger.LogError($"{DateTime.UtcNow} {this.ToString()}: {string.Join(" ",res.Errors!.Select(ex => ex.Message + " "))}");
                        throw new NullReferenceException();
                        
                    }
                }
            }
        }
    }
}
