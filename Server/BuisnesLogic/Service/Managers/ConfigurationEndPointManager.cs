using Amazon.Runtime.CredentialManagement;
using BuisnesLogic.Model.DeserializationModels;
using BuisnesLogic.ServicesInterface.ConfigurationEndPointsInterfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
namespace BuisnesLogic.Service.Managers
{
    public class ConfigurationEndPointManager<T> : IDeserializationConfigurationEndpoint<T> where T : EndPoint, new()
    {
        private readonly IEnumerable<IConfigurationSection> elements;
        public ConfigurationEndPointManager(IConfiguration configuration, string mainconfigurationsection)
        {
            elements = configuration.GetSection(mainconfigurationsection).GetChildren();
        }
        public T GetInfoEndPoint(string nameEndpoint)
        {
            if(string.IsNullOrEmpty(nameEndpoint)) throw new NullReferenceException(nameof(nameEndpoint));
            T info = new T();
            var section = elements.Where(x => x.Key.ToLower().Equals(nameEndpoint.ToLower())).SingleOrDefault();
            if (section is not null)
            {
                foreach (var el in section.GetChildren())
                {
                    foreach (var prop in info.GetType().GetProperties())
                    {
                        if (el.Key.ToLower().Equals(prop.Name.ToLower()))
                        {
                            prop.SetValue(info, el.Value);
                            break;
                        }
                    }
                }
            }
            return info;
        }
        public T[] GetAllEndPoints()
        {
            List<T> info = new List<T>();
            foreach (var sect in elements) info.Add(GetInfoEndPoint(sect.Key));
            return info.ToArray();
        }
    }
}
