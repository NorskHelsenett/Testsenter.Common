using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Shared.Common.Logic
{
    public static class XmlHelper
    {
        public static string GetInnerValueInXmlNode(this string msg, string start, string stop, bool replaceFnuffs = true)
        {
            var startIndex = msg.IndexOf(start, StringComparison.Ordinal);
            if (startIndex <= 0)
                return null;

            var length = msg.Substring(startIndex).IndexOf(stop, StringComparison.Ordinal);
            var element = msg.Substring(startIndex + start.Length, length - start.Length);
            if(replaceFnuffs)
                element = element.Replace("\"", "");

            element = element.Trim();

            return element;
        }

        public static string RemoveInnerValueInXmlNode(this string msg, string start, string stop)
        {
            var startIndex = msg.IndexOf(start, StringComparison.Ordinal);
            if (startIndex <= 0)
                return msg;

            var length = msg.Substring(startIndex).IndexOf(stop, StringComparison.Ordinal);
            var element = msg.Substring(startIndex + start.Length, length - start.Length);

            return msg.Remove(startIndex + start.Length, length - start.Length);
        }

        public static string ReplaceInnerValueInXmlNode(this string mime, string startString, string stopString, string replacement)
        {
            var startIndex = mime.IndexOf(startString, StringComparison.Ordinal) + startString.Length + (startString.Contains(">") ? 0 : 1);
            var stopIndex = mime.Substring(startIndex).IndexOf(stopString, StringComparison.Ordinal) + startIndex;

            mime = mime.Substring(0, startIndex) + replacement + mime.Substring(stopIndex);

            return mime;
        }

        public static T DeserializeFromXmlElement<T>(this XmlElement element)
        {
            var serializer = new XmlSerializer(typeof(T));
            return (T)serializer.Deserialize(new XmlNodeReader(element));
        }

        public static T DeserializeFromXElement<T>(this XElement element)
        {
            var serializer = new XmlSerializer(typeof(T));
            return (T)serializer.Deserialize(element.CreateReader());
        }

        public static T DeserializeFromXDocument<T>(XDocument element)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));

            using (var reader = element.Root.CreateReader())
            {
                return (T)xmlSerializer.Deserialize(reader);
            }
        }

        public static T DeserializeFromXmlString<T>(string xmlString)
        {
            var reader = new StringReader(xmlString);
            var serializer = new XmlSerializer(typeof(T));
            var instance = (T)serializer.Deserialize(reader);

            return instance;
        }
    }
}
