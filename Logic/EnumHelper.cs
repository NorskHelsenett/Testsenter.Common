using Shared.Common.Resources;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Shared.Common.Logic
{
    public static class EnumHelper
    {
        public static string ToDescription<T>(this T e) 
        {
            if(typeof(T).IsEnum)
                return GetEnumDescription(e as Enum);

            return GetDescription(typeof(T));
        }

        public static string ToAction<T>(this T e)
        {
            if (typeof(T).IsEnum)
                return GetEnumAction(e as Enum);

            return GetAction(typeof(T));
        }

        public static string GetDescription(Type type)
        {
            var descriptions = (DescriptionAttribute[])
                type.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (descriptions.Length == 0)
            {
                return null;
            }
            return descriptions[0].Description;
        }

        public static string GetAction(Type type)
        {
            var descriptions = (ActionAttribute[])
                type.GetCustomAttributes(typeof(ActionAttribute), false);

            if (descriptions.Length == 0)
            {
                return null;
            }
            return descriptions[0].Action;
        }

        public static T GetValueFromDescription<T>(string description)
        {
            return Enum.GetValues(typeof(T)).Cast<T>().FirstOrDefault(v => v.ToDescription() == description);
        }

        private static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute),
                false);

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (attributes != null &&
                attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }

        private static string GetEnumAction(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            ActionAttribute[] attributes =
                (ActionAttribute[])fi.GetCustomAttributes(
                typeof(ActionAttribute),
                false);

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (attributes != null &&
                attributes.Length > 0)
                return attributes[0].Action;
            else
                return value.ToString();
        }
    }
}
