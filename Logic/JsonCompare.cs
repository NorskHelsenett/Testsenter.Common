using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Shared.Common.Logic
{
    public static class JsonCompare
    {
        public static StringBuilder CompareObjects<T>(T source, T target, string keyNotFoundFormat = "Felt med navn {0} ble ikke funnet", string attributtValueNotEqualFormat = "Felt med navn {0} hadde forventet verdi {1} men nåværende verdi er {2}")
        {
            StringBuilder returnString = new StringBuilder();

            if (EqualsDefaultValue(source))
            {
                if (EqualsDefaultValue(target))
                    return returnString;

                returnString.AppendLine("Forventet at objekt var null, men nåværende verdi er ikke null");
                return returnString;
            }

            if (EqualsDefaultValue(target) && !EqualsDefaultValue(source))
            {
                returnString.AppendLine("Forventet at objekt var ikke-null, men nåværende verdi er null");
                return returnString;
            }

            try
            {
                return !IsArray(source) ?
                    CompareObjects(GetAs<T, JObject>(source), GetAs<T, JObject>(target), keyNotFoundFormat, attributtValueNotEqualFormat):
                    CompareArrays(GetAs<T, JArray>(source), GetAs<T, JArray>(target), string.Empty, keyNotFoundFormat, attributtValueNotEqualFormat);
            }
            catch (Exception)
            {
                
                throw;
            }
        }

        public static StringBuilder CompareObjects(JObject source, JObject target, string keyNotFoundFormat = "Felt med navn {0} ble ikke funnet", string attributtValueNotEqualFormat = "Felt med navn {0} hadde forventet verdi {1} men nåværende verdi er {2}")
        {
            StringBuilder returnString = new StringBuilder();
            foreach (KeyValuePair<string, JToken> sourcePair in source)
            {
                if (sourcePair.Value.Type == JTokenType.Object)
                {
                    if (target.GetValue(sourcePair.Key) == null)
                    {
                        returnString.Append(string.Format(keyNotFoundFormat, sourcePair.Key) + Environment.NewLine);
                    }
                    else
                    {
                        returnString.Append(CompareObjects(sourcePair.Value.ToObject<JObject>(), target.GetValue(sourcePair.Key).ToObject<JObject>()));
                    }
                }
                else if (sourcePair.Value.Type == JTokenType.Array)
                {
                    if (target.GetValue(sourcePair.Key) == null)
                    {
                        returnString.Append(string.Format(keyNotFoundFormat, sourcePair.Key) + Environment.NewLine);
                    }
                    else
                    {
                        returnString.Append(CompareArrays(sourcePair.Value.ToObject<JArray>(), target.GetValue(sourcePair.Key).ToObject<JArray>(), sourcePair.Key));
                    }
                }
                else
                {
                    JToken expected = sourcePair.Value;
                    var actual = target.SelectToken(sourcePair.Key);
                    if (actual == null)
                    {
                        returnString.Append(string.Format(keyNotFoundFormat, sourcePair.Key) + Environment.NewLine);
                    }
                    else
                    {
                        if (!JToken.DeepEquals(expected, actual))
                        {
                            var sourcePairValue = string.IsNullOrEmpty(sourcePair.Value.ToString()) ? "NULL" : sourcePair.Value.ToString();
                            var targetPropertyValue = string.IsNullOrEmpty(target.Property(sourcePair.Key).Value.ToString()) ? "NULL" : target.Property(sourcePair.Key).Value.ToString();

                            if(sourcePairValue != targetPropertyValue)
                                returnString.Append(string.Format(attributtValueNotEqualFormat, sourcePair.Key, sourcePairValue, targetPropertyValue) + Environment.NewLine);
                        }
                    }
                }
            }

            return returnString;
        }

        public static StringBuilder CompareArrays(JArray source, JArray target, string arrayName = "", string keyNotFoundFormat = "Felt med navn {0} ble ikke funnet", string attributtValueNotEqualFormat = "Felt med navn {0} hadde forventet verdi {1} men nåværende verdi er {2}")
        {
            var returnString = new StringBuilder();
            for (var index = 0; index < source.Count; index++)
            {
                var expected = source[index];
                if (expected.Type == JTokenType.Object)
                {
                    var actual = (index >= target.Count) ? new JObject() : target[index];
                    returnString.Append(CompareObjects(expected.ToObject<JObject>(), actual.ToObject<JObject>()));
                }
                else
                {

                    var actual = (index >= target.Count) ? "" : target[index];
                    if (!JToken.DeepEquals(expected, actual))
                    {
                        if (string.IsNullOrEmpty(arrayName))
                        {
                            returnString.Append("Index " + index + ": " + expected + " != " + actual + Environment.NewLine);
                        }
                        else
                        {
                            returnString.Append("Key " + arrayName + "[" + index + "]: " + expected + " != " + actual + Environment.NewLine);
                        }
                    }
                }
            }
            return returnString;
        }

        private static bool EqualsDefaultValue<T>(T value)
        {
            return EqualityComparer<T>.Default.Equals(value, default(T));
        }

        private static bool IsArray<T>(T value)
        {
            return value is Array;
        }

        private static TReturn GetAs<T, TReturn>(T value)
        {
            var asString = JsonConvert.SerializeObject(value);
            return JsonConvert.DeserializeObject<TReturn>(asString);
        }
    }
}
