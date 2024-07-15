using BuisnesLogic.Service;
using BuisnesLogic.Service.Clients;
using BuisnesLogic.Service.Managers;
using BuisnesLogic.ServicesInterface;
using BuisnesLogic.ServicesInterface.ClientsInterfaces;
using BuisnesLogic.ServicesInterface.ClientsInterfaces.MessageBrokers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
namespace BuisnesLogic.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddYandexCloud(this IServiceCollection services, IConfiguration configuration) => services.AddScoped<IYandexClient, YandexCloudClient>(arg => new YandexCloudClient(configuration["CloudConnection:ClientId"]!, configuration["CloudConnection:ClientSecret"]!, configuration["CloudConnection:ClientBucket"]!));
        public static IServiceCollection AddJwtManager(this IServiceCollection services) => services.AddTransient<JwtManager>();
        public static IServiceCollection AddMyValidations(this IServiceCollection services) => services.AddSingleton<IStrategyValidation, StrategyValidation>();
        /// <typeparam name="T"> This param is interface implementing IMessageBrokerClient<TModel> </typeparam>
        /// <typeparam name="TModel">Model</typeparam>
        public static IServiceCollection AddKafkaClient<TModel>(this IServiceCollection services, IConfiguration configuration, ILogger<KafkaClient<TModel>> logger) => services.AddSingleton<IMessageBrokerClient<TModel>, KafkaClient<TModel>>(arg => new KafkaClient<TModel>(logger, configuration["KafkaConfig:Host"]!, configuration["KafkaConfig:DefaultTopic"]!));
    }
}
