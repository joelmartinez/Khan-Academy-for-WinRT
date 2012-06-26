using System;
using System.Collections.Generic;

namespace KhanAcademy.Data
{
    public static class EnumerableExtensions
    {
        /// <summary>Method from here: http://stackoverflow.com/a/3907766 </summary>
        public static IEnumerable<T> Flatten<T>(this IEnumerable<T> source,Func<T, IEnumerable<T>> childrenSelector)
        {
            if (source == null) yield break;

            foreach (var item in source)
            {
                yield return item;
                foreach (var child in childrenSelector(item).Flatten(childrenSelector))
                {
                    yield return child;
                }
            }
        }
    }
}
