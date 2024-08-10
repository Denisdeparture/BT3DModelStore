using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnesLogic.ServicesInterface.ClientsInterfaces.MessageBrokers
{
    public interface IConsumeClient<TModel>
    {
        public ObservableCollection<TModel> Models { get; set; }
        public void Consume(uint consuming_time,CancellationTokenSource cts,string? topic = null);
    }
}
