using BuisnesLogic.Model.ServiceResultModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnesLogic.ServicesInterface.ClientsInterfaces.MessageBrokers
{
    public interface IMessageBrokerClient<TModel> : IProduceClient<TModel>, IConsumeClient<TModel>
    {
    }
}
