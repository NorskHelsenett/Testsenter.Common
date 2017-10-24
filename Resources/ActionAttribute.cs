using System;

namespace Shared.Common.Resources
{
    public class ActionAttribute : Attribute
    {
        public string Action { get; set; }
        public ActionAttribute(string action)
        {
            Action = action;
        }
    }
}
