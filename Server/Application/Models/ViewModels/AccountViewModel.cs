using DomainModel.Entity;
using Microsoft.AspNetCore.Identity;
using Endpoint = BuisnesLogic.Model.DeserializationModels.EndPoint;
namespace Application.Models.ViewModels
{
    public class AccountViewModel 
    {
        public User About { get; set; } = null!;
        public IList<string>? Roles { get; set; } = new List<string>();
        public IList<Endpoint>? Action { get; set; } = new List<Endpoint>();
        public IList<Product>? Purchases { get; set; } = new List<Product>();

    }
}
