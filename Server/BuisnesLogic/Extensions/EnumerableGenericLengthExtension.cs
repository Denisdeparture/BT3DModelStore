using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnesLogic.Extensions
{
    public static class EnumerableGenericLengthExtension
    {
        public static int GetLengthFromThisCollection<T>(this IEnumerable<T> collection, string propertyLenghtinthisCollection)
        {
            if(string.IsNullOrEmpty(propertyLenghtinthisCollection)) throw new ArgumentNullException();
            var prop = collection.GetType().GetProperty(propertyLenghtinthisCollection) ?? throw new NullReferenceException("this property is not exist");
            var res = prop.GetValue(collection) as int? ?? throw new InvalidCastException("result is not number type");
            return res;
        }
       
    }
}
