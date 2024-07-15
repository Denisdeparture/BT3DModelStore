using BuisnesLogic.Models.SerializationModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnesLogic.ServicesInterface.ConfigurationEndPointsInterfaces
{
    public interface IConfigurationEndpointOperation<in T> where T : EndPoint
    {
        public void Add(T endpointinfo);
        public void Remove(T endpointinfo);

    }
}
