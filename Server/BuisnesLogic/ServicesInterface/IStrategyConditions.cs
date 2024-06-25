using BuisnesLogic.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnesLogic.ServicesInterface
{
    public interface IStrategyConditions
    {
        public StrategyResult<T> StrategyCondition<T>(IList<Predicate<T>> methodscondition, T obj);
    }
}
