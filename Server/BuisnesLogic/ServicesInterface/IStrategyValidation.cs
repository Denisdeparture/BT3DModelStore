using BuisnesLogic.Model.ServiceResultModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnesLogic.ServicesInterface
{
    public interface IStrategyValidation
    {
        public StrategyResult<T> StrategyCondition<T>(IList<Predicate<T>> methodscondition, T obj);
    }
}
