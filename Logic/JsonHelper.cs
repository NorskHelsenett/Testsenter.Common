using System;
using Newtonsoft.Json;

namespace Shared.Common.Logic
{
    public static class JsonHelper
    {
        public static string ToJson(this object value)
        {
            return JsonConvert.SerializeObject(value);
        }

        [Obsolete("Consider using JsonCompare (in Common)")]
        public static bool CompareSerialized<T>(T a, T b)
        {
            return a.ToJson() == b.ToJson();
        }

        public static T Clone<T>(this T source)
        {
            var serialized = JsonConvert.SerializeObject(source);
            return JsonConvert.DeserializeObject<T>(serialized);
        }
    }
}
