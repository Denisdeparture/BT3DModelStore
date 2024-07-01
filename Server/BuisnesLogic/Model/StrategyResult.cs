using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnesLogic.Model
{
    public class StrategyResult<T> : IDisposable
    {
        public T? Object { get; set; } 
        public bool Completed { get; set; }

        public void Dispose()
        {
            GC.SuppressFinalize(Object!);
            GC.Collect();
        }
        public static implicit operator bool(StrategyResult<T> result) => result.Completed;
    }
}
