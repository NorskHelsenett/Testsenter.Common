using System;
using System.Collections.Generic;
using System.Linq;

namespace Shared.Common.Logic
{
    public static class TextHelper
    {
        public static string AsCommaseparatedText(this IEnumerable<string> items)
        {
            var asList = items.ToList();
            var s = "";
            for (int i = 0; i < asList.Count(); i++)
            {
                s += asList[i];
                if (i != (asList.Count - 1))
                    s += ", ";
            }

            return s;
        }

        public static bool SomewhatEquals(this string a, string b)
        {
            if (a == null && b == null) return true;
            if (a == null || b == null) return false;
            a = a.ToLower();
            b = b.ToLower();
            if (a == b) return true;
            if (a.Contains(b)) return true;
            if (b.Contains(a)) return true;
            if (a.Split(' ').All(w => b.Contains(w))) return true;
            if (b.Split(' ').All(w => a.Contains(w))) return true;
            return false;
        }

        public static bool CompareCaseInsensitive(this string a, string b)
        {
            return string.Equals(a, b, StringComparison.CurrentCultureIgnoreCase);
        }
    }
}
