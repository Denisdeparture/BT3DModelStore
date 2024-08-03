using BuisnesLogic.Model.ServiceResultModels;
using BuisnesLogic.ServicesInterface;

namespace BuisnesLogic.Service
{
    public class StrategyValidation : IStrategyValidation
    {
        public StrategyResult<T> StrategyCondition<T>(IList<Predicate<T>> methodscondition, T obj)
        {
            int count = 0;
            foreach (var methods in methodscondition)
            { 
                if (!methods(obj))  return new StrategyResult<T>() { Completed = false, Object = obj, NumberConditionCounter = count }; 
                count++;
            }
            return new StrategyResult<T>() { Completed = true, Object = obj};
        }
    }
}
