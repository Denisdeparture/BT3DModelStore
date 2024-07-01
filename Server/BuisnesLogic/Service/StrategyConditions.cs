using BuisnesLogic.Model;
using BuisnesLogic.ServicesInterface;

namespace Application
{
    public class StrategyConditions : IStrategyConditions
    {
        public StrategyResult<T> StrategyCondition<T>(IList<Predicate<T>> methodscondition, T obj)
        {
            foreach (var methods in methodscondition) if (!methods(obj)) { return new StrategyResult<T>() { Completed = false, Object = obj }; }
            return new StrategyResult<T>() { Completed = true, Object = obj};
        }
    }
}
