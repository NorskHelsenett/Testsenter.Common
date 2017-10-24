using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Shared.Common.Logic
{
    public static class TimeHelper
    {
        public static DateTime GetNorwegianTime()
        {
            return TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.Utc, TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time"));
        }

        public static DateTime ToNorwegianTime(this DateTime t)
        {
            if (t.Kind == DateTimeKind.Unspecified)
                return t;

            return TimeZoneInfo.ConvertTime(t, TimeZoneInfo.Utc, TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time"));
        }

        public static bool SomewhatEquals(this DateTime a, DateTime b)
        {
            var diff = Math.Abs(a.Subtract(b).TotalSeconds);
            return diff <= new TimeSpan(0, 10, 0).TotalSeconds;
        }

        public static string ToNorwegianText(DateTime? t)
        {
            if(t == null)
                return "n/a";

            return ToNiceText(ToNorwegianTime(t.Value));
        }

        public static string ToNiceText(DateTime t)
        {
            return t.ToString("dd/MM/yy, kl HH:mm");
        }

        public static DateTime GetLocalTime()
        {
            return DateTime.Now.ToLocalTime();
        }

        public static DateTime ParseDateTimeFromEbms(string dateAsText, string format = "dd.MM.yyyy HH:mm:ss")
        {
            var multiplespacesRegex = new Regex("[ ]{2,}", RegexOptions.None);
            var trimmed = dateAsText.Replace(Environment.NewLine, " ").Trim();
            trimmed = multiplespacesRegex.Replace(trimmed, " ");
            
            return DateTime.ParseExact(trimmed, format, CultureInfo.InvariantCulture);
        }
    }
}
