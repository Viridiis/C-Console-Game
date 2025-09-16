using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace System.Collections.Generic
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class ExtraGenericEnumerableExtensions
    {
        public static int IndexOf<T>(this IEnumerable<T> enumerable, T item)
        {
            if(enumerable is IList<T> c)
            {
                return c.IndexOf(item);
            }
            return IndexOf(enumerable, item, null);
        }

        public static int IndexOf<T>(this IEnumerable<T> enumerable, T item, IEqualityComparer<T> comparer)
        {
            ArgumentNullException.ThrowIfNull(enumerable);
            if(comparer is null) comparer = EqualityComparer<T>.Default;
            int i = 0;
            foreach(T t in enumerable)
            {
                if(comparer.Equals(t, item)) return i;
                i++;
            }
            return -1;
        }
        
        public static int FindIndexOf<T>(this IEnumerable<T> enumerable, Predicate<T> predicate)
        {
            ArgumentNullException.ThrowIfNull(enumerable);
            ArgumentNullException.ThrowIfNull(predicate);
            int i = 0;
            foreach (T t in enumerable)
            {
                if(predicate(t)) return i;
                i++;
            }
            return -1;
        }
    }
}
