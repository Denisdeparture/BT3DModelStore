using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnesLogic.Model.ServiceResultModels
{
    public class StrategyResult<T>
    {
        public T? Object { get; set; }
        public bool Completed { get; internal set; }
        public int NumberConditionCounter { get; internal set; }

        public static implicit operator bool(StrategyResult<T> result) => result.Completed;
    }
}

