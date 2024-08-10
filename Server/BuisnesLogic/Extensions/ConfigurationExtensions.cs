using BuisnesLogic.Service.Clients;
using BuisnesLogic.Service.Managers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BuisnesLogic.Extensions
{
    public static class ConfigurationExtensions
    {
        public static IConfigurationBuilder AddPantryStorage(this IConfigurationBuilder configurationBuilder, string token, string bucket)
        {
            using (ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole()))
            {
                ILogger<PantrySecretsProviderManager> loggerForConfig = factory.CreateLogger<PantrySecretsProviderManager>();
                if (string.IsNullOrWhiteSpace(token) | string.IsNullOrWhiteSpace(bucket)) throw new ArgumentNullException("Token or bucket name was incorrect");
                var source = new PantrySecretsProviderManager(token, bucket, loggerForConfig);
                var res = source.GetData();
                configurationBuilder.AddJsonStream(res);
                return configurationBuilder;
            }
        }
        public static string GetServerAddres(this IConfiguration configuration) => string.Format("{0}://{1}", configuration["ServerPath:Protocol"]!, configuration["ServerPath:Host"]);
        public static string GetAlternativeServerAddres(this IConfiguration configuration) => string.Format("{0}://{1}", configuration["ServerPath:Protocol"]!, configuration["ServerPath:HostAlternative"]);
    }
}
