using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PJSK.Utility.Common
{
    public static class Constants
    {
        public class DateTime
        {
            public struct IANATimeZones
            {
                public const string UTC = "Etc/UTC";

                // CA
                public const string Toronto = "America/Toronto";

                // JP
                public const string Japan = "Asia/Tokyo";
                public const string Tokyo = "Asia/Tokyo";
            }

            public struct DateTimePatterns
            {
                public const string Date = "yyyy-MM-dd";
                public const string DateTime = "yyyy-MM-dd HH\\:mm\\:ss";

                public struct DotNet
                {
                    public const string ISO8601 = "yyyy-MM-ddTHH\\:mm\\:sszzz";
                    public const string ISO8601_Precise = "yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz";
                }

                public struct NodaTime
                {
                    public const string ISO8601_Offset_HHmm = "yyyy-MM-ddTHH\\:mm\\:sso<+HH:mm>";
                    public const string ISO8601_Offset_HHmm_Precise = "yyyy-MM-ddTHH\\:mm\\:ss.fffffffo<+HH:mm>";
                }
            }
        }
    }
}
