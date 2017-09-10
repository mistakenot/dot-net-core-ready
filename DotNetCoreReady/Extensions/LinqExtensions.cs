using System;
using System.Collections.Generic;
using System.Linq;

namespace DotNetCoreReady.Extensions
{
    public static class LinqExtensions
    {
        public static IEnumerable<T> DistinctBy<T, S>(this IEnumerable<T> enumerable, Func<T, S> selector)
        {
            var set = new HashSet<S>();
            return enumerable.Where(t => set.Add(selector(t)));
        }
    }
}