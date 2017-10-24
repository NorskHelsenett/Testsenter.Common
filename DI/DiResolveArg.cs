using System;

namespace Shared.Common.DI
{
    public class DiResolveArg
    {
        public Type Type { get; set; }
        public string Name { get; }

        public DiResolveArg(Type type, string name = "")
        {
            Type = type;
            Name = name;
        }

        public DiResolveArg(Type type, Enum name)
        {
            Type = type;
            Name = name.ToString();
        }

        public static DiResolveArg Create<T>(string name = "")
        {
            return new DiResolveArg(typeof(T), name);
        }
    }
}
