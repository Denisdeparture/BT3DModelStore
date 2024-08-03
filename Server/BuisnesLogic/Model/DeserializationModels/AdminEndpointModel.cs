using Microsoft.AspNetCore.Html;

namespace BuisnesLogic.Models.SerializationModels
{
    public class AdminEndpointModel : EndPoint
    {
        public string Action { get; set; } = null!;
        public string PathFromMarking { get; set; } = string.Empty;

    }
}
