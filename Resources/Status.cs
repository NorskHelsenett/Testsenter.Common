using System;
// ReSharper disable InconsistentNaming

namespace Shared.Common.Resources
{
    public interface IStatusEntity
    {
        Status GetStatus();
    }

    public enum Status
    {
        onGoing = 0,
        failed = 2,
        success = 3,
        stopped = 4,
        waiting = 5
    }

    public static class StatusHelper
    {
        public static string GetCssStatusAsString(int statusEnum)
        {
            var x = (Status)statusEnum;
            switch (x)
            {
                case Status.failed:
                    return "label-danger";

                case Status.onGoing:
                    return "label-warning";

                case Status.stopped:
                    return "label-primary";

                case Status.success:
                    return "label-success";

                case Status.waiting:
                    return "label-warning";

                default:
                    return "";
            }
        }

        public static string GetStatusAsString(int statusEnum)
        {
            return GetStatusAsString((Status)statusEnum);
        }

        public static string GetStatusAsString(Status StatusEnum)
        {
            try
            {
                var x = StatusEnum;
                switch (x)
                {
                    case Status.failed:
                        return "Feilet";

                    case Status.onGoing:
                        return "Pågående";

                    case Status.stopped:
                        return "Stoppet";

                    case Status.success:
                        return "Ferdig";

                    case Status.waiting:
                        return "Allokerer ressurser";
                }
                return string.Empty;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }
}
