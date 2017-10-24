using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Shared.Common.Logic
{
    public static class ListHelper
    {
        public static ValT GetOrDefault<KeyT, ValT>(this Dictionary<KeyT, ValT> dictionary, KeyT key, ValT @default = default(ValT))
        {
            return dictionary.ContainsKey(key) ? dictionary[key] : @default;
        }

        public static bool ContainsAllKeys<T>(this Dictionary<string, T> dictionary, params string[] keys)
        {
            return keys.All(dictionary.ContainsKey);
        }

        public static bool ContainsAnyKeys<T>(this Dictionary<string, T> dictionary, params string[] keys)
        {
            return keys.Any(dictionary.ContainsKey);
        }

        public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> root)
        {
            return root.SelectMany(node => node);
        }

        public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> values, Func<T, TKey> property)
        {
            return values.GroupBy(property).Select(g => g.FirstOrDefault<T>());
        }

        public static string Stringify(this IEnumerable<int> items)
        {
            return Stringify(items.Select(y => y.ToString()));
        }

        public static string Stringify(this IEnumerable<string> items)
        {
            if(items == null)
                return String.Empty;

            var s = new StringBuilder();
            var separator = ", ";

            foreach (var item in items)
            {
                s.Append(item);
                s.Append(separator);
            }

            var asStr = s.ToString();
            if (asStr.EndsWith(separator))
                asStr = asStr.Substring(0, asStr.Length - separator.Length);

            return asStr;
        }
    }
}
