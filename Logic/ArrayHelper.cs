using System.Collections.Generic;
using Newtonsoft.Json;

namespace Shared.Common.Logic
{
    public static class ArrayHelper
    {
        public static void Add<T>(this T[] array, T newvalue)
        {
            if (array.Length == 0)
            {
                array = new T[] { newvalue };
                return;
            }

            var tmp = new T[array.Length + 1];
            for (int i = 0; i < array.Length; i++)
                tmp[i] = array[i];

            tmp[tmp.Length - 1] = newvalue;

            array = tmp;
        }

        public static List<T> Copy<T>(this List<T> master)
        {
            var asJson = JsonConvert.SerializeObject(master);
            return JsonConvert.DeserializeObject<List<T>>(asJson);
        }

        public static string Concatenate(IEnumerable<string> values)
        {
            var s = "";
            foreach (var value in values)
                s += value;

            return s;
        }
    }
}
