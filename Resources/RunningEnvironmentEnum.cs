using System.ComponentModel;

namespace Shared.Common.Resources
{
    public enum RunningEnvironmentEnum
    {
        [Description("Localhost")]
        Localhost = 1000,

        [Description("Staging")]
        Staging = 2000,

        [Description("Prod")]
        Prod = 3000,

        [Description("Ingen")]
        Ingen = 5000,

        Ugyldig = 1000000
    }
}
