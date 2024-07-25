using ApplicationInfrastructure;
using BuisnesLogic.ServicesInterface;
using DataBase.AppDbContexts;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework.Internal;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;
using Microsoft.Extensions.Logging;
using Application.Controllers;
using BuisnesLogic.Service;
using BuisnesLogic.Model.ServiceResultModels;
using BuisnesLogic.ServicesInterface.ClientsInterfaces;
namespace WebServer.Tests
{
    public class ProductOperationTest
    {

#pragma warning disable NUnit1032 
        private AdminController controller;
        private IConfiguration Configuration;
        private Mock<IYandexClient>? mockYandexClient;
        private IStrategyValidation Strategy;
        private MainDbContext data;
        private Mock<ILogger<Program>> mocklogger;
#pragma warning restore NUnit1032
        [SetUp]
        public void SetDi()
        {

            mockYandexClient = new Mock<IYandexClient>();
            mocklogger = new Mock<ILogger<Program>>();
            // test data p.s url is not exist
            bool isCorrect = false;
            string url = "";
            using (var stream = new FileStream(@"C:\TestData\testfile.txt", FileMode.Open))
            {
                mockYandexClient.Setup(fakeobjfunc => fakeobjfunc.AddModel(stream, "", stream.Name)).Returns(new Task<AwsActionResultModel>(() => new AwsActionResultModel() { isCorrect = isCorrect, resultUrlFromModel = url }));
            }
            var initialization = new Dictionary<string, string>()
            {
                {"PathFromStorage", "Model"}
            };
            Configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(initialization!)
                .Build();
            Strategy = new StrategyValidation();
            data = new MainDbContext("Server=PC;Database=ApplicationDataBase;Trusted_Connection=True;TrustServerCertificate=True;Connection Timeout=60");
            controller = new AdminController(data, mockYandexClient.Object, Configuration, Strategy, mocklogger.Object);
        }
        [Test]
        public void Test_Add_When_ModelNull()
        {
            // Arrange
            var task = controller.AddProductPost(null);
            task.Wait();
            // Act
            var res = task.Result;
        }
        [Test]
        public void Test_Add_When_ModelParametNull()
        {
            // Arrange
            var task = controller.AddProductPost(new ProductViewModel());
            task.Wait();
            // Act
            var res = task.Result;
        }
        [Test]
        public void Test_Add_When_ModelStringParametNull()
        {
            // Arrange
            using (var stream = new FileStream(@"C:\TestData\testfile.txt", FileMode.Open))
            {
                var task = controller.AddProductPost(new ProductViewModel()
                {
                    Price = 12,
                    Description = "",
                    Name = string.Empty,
                    Quantity = 15,
                    Model = new FormFile(stream, stream.Length, stream.Length, stream.Name, stream.Name)
                });
                task.Wait();
                // Act
                var res = task.Result;
            }
        }
        [Test]
        public void Test_Delete()
        {

        }

        //[Test]
        //public void Test_Update() { }
    }
}
