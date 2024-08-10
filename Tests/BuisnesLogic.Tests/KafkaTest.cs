using BuisnesLogic.ServicesInterface.ClientsInterfaces.MessageBrokers;
using DomainModel.Entity;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq.Language.Flow;
using BuisnesLogic.Service.Clients;
using Microsoft.Extensions.Logging;
using BuisnesLogic.Service.Managers;
namespace BuisnesLogic.Tests
{
    public class KafkaTest
    {
        private  IMessageBrokerClient<Product> _kafkaclient;
        private readonly ILogger<KafkaClient<Product>> _logger;
        public KafkaTest()
        {
            using (ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole()))
            _logger = factory.CreateLogger<KafkaClient<Product>>();
            var cts = new CancellationTokenSource();
        }
        [Fact]
        public void Test_When_Args_For_Ctor_Is_Null()
        {
            try
            {
                //Arange
                // Act
                _kafkaclient = new KafkaClient<Product>(null, "", "", "", "");
            }
            catch(ArgumentNullException) 
            {
                // Assert
                Assert.True(true);
            }
            Assert.True(false);
        }
        [Fact]
        public void Test_Produce_When_Model_Is_Null()
        {
            try
            {
                //Arange
                _kafkaclient = new KafkaClient<Product>(_logger, "localhost:9092", "test");
                // Act
                _kafkaclient.Produce(null, null);
            }
            catch (ArgumentNullException)
            {
                // Assert
                Assert.True(true);
            }
            Assert.True(false);
        }
        [Fact]
        public void Test_Kafka_With_Out_Connection()
        {
            //Arange
            _kafkaclient = new KafkaClient<Product>(_logger, "localhost:9092", "test", "webservertest", "consumertest");
            var cts = new CancellationTokenSource();
            var product = new Product()
            {
                Id = 5,
                Name = "Test",
                Url = "https://storage.yandexcloud.net/ycbucketbt3dmodel/Model/bed_minecraft.glb",
                Price = 123.9021
            };
            // Act
            try
            {
                _kafkaclient.Produce(product, cts, "test");
            }
            catch
            {
                // Assert
                Assert.True(true);
            }
            Assert.True(false);
        }

    }
}
