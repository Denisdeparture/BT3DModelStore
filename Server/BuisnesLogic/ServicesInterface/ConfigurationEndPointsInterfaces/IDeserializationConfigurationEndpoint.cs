using BuisnesLogic.Model.DeserializationModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnesLogic.ServicesInterface.ConfigurationEndPointsInterfaces
{
    public interface IDeserializationConfigurationEndpoint<out T> where T : EndPoint
    {
        public T GetInfoEndPoint(string nameEndpoint);
        public T[] GetAllEndPoints();
    }
}
