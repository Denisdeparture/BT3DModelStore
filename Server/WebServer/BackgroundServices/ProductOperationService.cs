using BuisnesLogic.ConstStorage;
using BuisnesLogic.ServicesInterface.ClientsInterfaces.MessageBrokers;
using Contracts;
using DomainModel;
using Infrastructure;
using Org.BouncyCastle.Utilities;
using System.Collections.Specialized;
using System.IO;

namespace WebServer.BackgroundServices
{
    public class ProductOperationService : BackgroundService
    {
        private readonly IConsumeClient<ProductContractModelJson> _messageBroker;
        private readonly ILogger<Program> _logger;
        private readonly IServiceProvider _serviceProvider;
        public ProductOperationService(ILogger<Program> logger, IMessageBrokerClient<ProductContractModelJson> client, IServiceProvider serviceProvider)
        {
            _messageBroker = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _messageBroker.Models.CollectionChanged += Add_Product;
        }
        private void Add_Product(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                Register(e.NewItems!.Cast<ProductContractModelJson>());
            }
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            CancellationTokenSource _tokenSource = new CancellationTokenSource();
            var factoryTask = new TaskFactory(stoppingToken);
            var tasks = new[]
            {
              factoryTask.StartNew(() => _messageBroker.Consume(20, _tokenSource, KafkaTopics.AddProductTopic)),
              factoryTask.StartNew(() => _messageBroker.Consume(20, _tokenSource, KafkaTopics.AddProductTopic))
            };
            return Task.CompletedTask;
        }
        private async void Register(IEnumerable<ProductContractModelJson> products)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                foreach (var product in products)
                {
                    MemoryStream stream = new MemoryStream();
                    stream.Write(product.FileInBytes!, 0, product.FileInBytes!.Length);
                    await scope.ServiceProvider.GetRequiredService<IProductOperation>().CreateProductAsync(product, stream, product.FileName!);
                }
            }
              
        }
     }
}

