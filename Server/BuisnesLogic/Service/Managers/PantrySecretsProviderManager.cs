using BuisnesLogic.ServicesInterface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Net.Http.Json;
namespace BuisnesLogic.Service.Managers
{
    public class PantrySecretsProviderManager : ISecretsProvider
    {
        //use put
        private readonly string BaseUrl = "https://getpantry.cloud/apiv1/pantry";
        private readonly ILogger<PantrySecretsProviderManager> _logger;
        public PantrySecretsProviderManager(string token_or_id, string namebucket, ILogger<PantrySecretsProviderManager> logger)
        {
            BaseUrl += string.Concat("/", token_or_id, "/", "basket", "/", namebucket);
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public Stream GetData()
        {
            HttpResponseMessage? resp = null;
            string error = string.Empty;

            using (var httpclient = new HttpClient())
            {
                try
                {
                    var task = httpclient.GetAsync(BaseUrl);
                    task.Wait();
                    resp = task.Result;
                }
                catch (HttpRequestException ex)
                {
                    error = DateTime.UtcNow + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace;
                }
                catch (Exception ex)
                {
                    error = DateTime.UtcNow + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace;
                }
                finally
                {
                    _logger.LogError(error);
                    httpclient.Dispose();
                }

            }
            if (resp is null) throw new NullReferenceException("Resp is null");
            var data = resp.Content.ReadAsStreamAsync();
            data.Wait();
            return data.Result;
        }
    }
}