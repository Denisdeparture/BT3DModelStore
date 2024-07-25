using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using BuisnesLogic.Model.ServiceResultModels;
using BuisnesLogic.ServicesInterface.ClientsInterfaces.MessageBrokers;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
namespace BuisnesLogic.Service.Clients
{
   
    public sealed partial class KafkaClient<TModel> : IMessageBrokerClient<TModel>
    {
        private readonly ProducerConfig _producerConf;
        private readonly ConsumerConfig _consumeConf;
        private readonly ILogger<KafkaClient<TModel>> _logger;
        public string BaseTopic { get; init; }
        public ObservableCollection<TModel> Models { get; set; }

        // Решение с int временное
        public KafkaClient(ILogger<KafkaClient<TModel>> logger, string host, string basetopic, string clientid = "webserverclient", string groupid = "consumerGroupTest")
        {
            _producerConf = new ProducerConfig()
            {
                BootstrapServers = host,
                Acks = Acks.All,
                Partitioner = Confluent.Kafka.Partitioner.ConsistentRandom
            };
            _consumeConf = new ConsumerConfig()
            {
                BootstrapServers = host,
                ClientId = clientid,
                GroupId = groupid,
            };
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            BaseTopic = basetopic;
            Models = new ObservableCollection<TModel>();
        }
        public async Task<ProduceResultModel> Produce(TModel model, CancellationTokenSource cts, string? topic = null)
        {
            var message = new Message<string, string>()
            {
                Key = new Guid().ToString(),
                Value = JsonSerializer.Serialize(model)
            };
            using (var producer = new ProducerBuilder<string, string>(_producerConf).SetKeySerializer(Serializers.Utf8).Build())
            {
                try
                {
                    await producer.ProduceAsync(topic ?? BaseTopic, message, cts.Token);
                    return new ProduceResultModel() { Success = true };
                }
                catch (Exception ex)
                {
                     string msg = "producer got this error: " + ex.Message + Environment.NewLine + ex.StackTrace;
                    _logger.LogError(msg);
                    return new ProduceResultModel() { Success = false, ErrorDescription = msg, Exception = ex };
                }
            }
        }
        public void Consume(int consuming_time, CancellationTokenSource cts, string? topic = null)
        {
            using (var consumer = new ConsumerBuilder<string, string>(_consumeConf).Build())
            {
                consumer.Subscribe(topic ?? BaseTopic);
                
                while (!cts.IsCancellationRequested)
                {
                    TModel? data = default;
                    try
                    {
                        var res = consumer.Consume(TimeSpan.FromSeconds(consuming_time));
                        Task.Delay(TimeSpan.FromSeconds(consuming_time));
                        if (res is not null)
                        {
                            data = JsonSerializer.Deserialize<TModel>(res.Message.Value);
                            _logger.LogInformation(DateTime.UtcNow + this.ToString() + " User Added");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(DateTime.UtcNow + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace);
                    }
                    const string jsonError = "data can`t deserialize from json";
                    if(data is not null)
                    {
                        AddModel(data);
                    }
                  
                }
            }

        }

    }
    public partial class KafkaClient<TModel>
    {
        private void AddModel(TModel model)
        {
            lock (Models)
            {
                Models.Add(model);
            }
        }
    }
}
