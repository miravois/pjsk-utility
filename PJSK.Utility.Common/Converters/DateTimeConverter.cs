using NodaTime;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PJSK.Utility.Common.Converters
{
    public static class DateTimeConverter
    {
        public static string ConvertJapanToUTC_GetString(string strJapan)
        {
            if (!DateTime.TryParse(strJapan, out _)) { return null; }

            var fromDateTimeString = strJapan;
            var fromPattern = (string)null;
            var fromTimeZoneIANA = Constants.DateTime.IANATimeZones.Japan;
            var toPattern = Constants.DateTime.DateTimePatterns.NodaTime.ISO8601_Offset_HHmm;
            var toTimeZoneIANA = Constants.DateTime.IANATimeZones.UTC;
            ConvertTimeZone(fromDateTimeString, fromPattern, fromTimeZoneIANA, toPattern, toTimeZoneIANA, out _, out _, out var strUTC);

            return strUTC;
        }
        public static string ConvertJapanToUTC_GetString(DateTime? dtJapan)
        {
            if (!dtJapan.HasValue) { return null; }
            return ConvertJapanToUTC_GetString(dtJapan.Value.ToString());
        }

        public static DateTime? ConvertUTCToJapan_GetDateTime(string strUTC)
        {
            if (!DateTime.TryParse(strUTC, out _)) { return null; }

            var fromDateTimeString = strUTC;
            var fromPattern = (string)null;
            var fromTimeZoneIANA = Constants.DateTime.IANATimeZones.UTC;
            var toPattern = Constants.DateTime.DateTimePatterns.DateTime;
            var toTimeZoneIANA = Constants.DateTime.IANATimeZones.Japan;
            ConvertTimeZone(fromDateTimeString, fromPattern, fromTimeZoneIANA, toPattern, toTimeZoneIANA, out var toZonedDateTime, out _, out var _);

            return toZonedDateTime == null ? null : toZonedDateTime.Value.ToDateTimeUnspecified();
        }
        public static string ConvertUTCToJapan_GetString(string strUTC)
        {
            if (!DateTime.TryParse(strUTC, out _)) { return null; }

            var fromDateTimeString = strUTC;
            var fromPattern = (string)null;
            var fromTimeZoneIANA = Constants.DateTime.IANATimeZones.UTC;
            var toPattern = Constants.DateTime.DateTimePatterns.DateTime;
            var toTimeZoneIANA = Constants.DateTime.IANATimeZones.Japan;
            ConvertTimeZone(fromDateTimeString, fromPattern, fromTimeZoneIANA, toPattern, toTimeZoneIANA, out _, out _, out var strJapan);

            return strJapan;
        }

        public static DateTime? ConvertUTCToLocal_GetDateTime(string strUTC)
        {
            if (!DateTime.TryParse(strUTC, out _)) { return null; }

            var fromDateTimeString = strUTC;
            var fromPattern = (string)null;
            var fromTimeZoneIANA = Constants.DateTime.IANATimeZones.UTC;
            var toPattern = Constants.DateTime.DateTimePatterns.DateTime;
            var toTimeZoneIANA = (string)null;
            ConvertTimeZone(fromDateTimeString, fromPattern, fromTimeZoneIANA, toPattern, toTimeZoneIANA, out var toZonedDateTime, out _, out var _);

            return toZonedDateTime == null ? null : toZonedDateTime.Value.ToDateTimeUnspecified();
        }
        public static string ConvertUTCToLocal_GetString(string strUTC)
        {
            if (!DateTime.TryParse(strUTC, out _)) { return null; }

            var fromDateTimeString = strUTC;
            var fromPattern = (string)null;
            var fromTimeZoneIANA = Constants.DateTime.IANATimeZones.UTC;
            var toPattern = Constants.DateTime.DateTimePatterns.DateTime;
            var toTimeZoneIANA = (string)null;
            ConvertTimeZone(fromDateTimeString, fromPattern, fromTimeZoneIANA, toPattern, toTimeZoneIANA, out _, out _, out var strLocal);

            return strLocal;
        }
        
        #region Helper Methods
        /// <summary>
        /// Convert date-time string to be in another time zone.
        /// </summary>
        /// <param name="fromDateTimeString">The date-time string to be converted from.</param>
        /// <param name="fromPattern">The format/pattern of fromDateTimeString. Consider to use Constants.DateTimePatterns.DotNet.</param>
        /// <param name="fromTimeZoneIANA">The IANA time zone name of fromDateTimeString. Consider to use Constants.IANATimeZones. If null will set to LOCAL.</param>
        /// <param name="toTimeZoneIANA">The IANA time zone name of toZonedDateTime and toDateTimeString. Consider to use Constants.IANATimeZones. If null will set to LOCAL.</param>
        /// <param name="toPattern">The format/pattern of toDateTimeString. Consider to use Constants.DateTimePatterns.NodaTime.</param>
        /// <param name="toDateTimeOffset">The output of conversion, in type of DateTimeOffset?.</param>
        /// <param name="toDateTimeString">The output of conversion, in type of string with format/pattern of toPattern.</param>
        /// <param name="cultureInfo">CultureInfo to be used. If null will set to InvariantCulture.</param>
        private static void ConvertTimeZone(string fromDateTimeString, string fromPattern, string fromTimeZoneIANA, string toPattern, string toTimeZoneIANA,
            out ZonedDateTime? toZonedDateTime, out DateTimeOffset? toDateTimeOffset, out string toDateTimeString,
            CultureInfo cultureInfo = null)
        {
            toZonedDateTime = null;
            toDateTimeOffset = null;
            toDateTimeString = null;
            if (string.IsNullOrWhiteSpace(fromDateTimeString)) { return; }

            var fromDateTime = DateTime.MinValue;
            var isDateTimeParseSuccessful = string.IsNullOrWhiteSpace(fromPattern)
                ? DateTime.TryParse(fromDateTimeString, out fromDateTime)
                : DateTime.TryParseExact(fromDateTimeString, fromPattern, cultureInfo, DateTimeStyles.None, out fromDateTime);
            var fromDateTimeNullable = !isDateTimeParseSuccessful ? (DateTime?)null : fromDateTime;
            ConvertTimeZone(fromDateTimeNullable, fromTimeZoneIANA, toPattern, toTimeZoneIANA, out toZonedDateTime, out toDateTimeOffset, out toDateTimeString, cultureInfo);
        }

        /// <summary>
        /// Convert date-time string to be in another time zone.
        /// </summary>
        /// <param name="fromDateTime">The DateTime to be converted from.</param>
        /// <param name="fromTimeZoneIANA">The IANA time zone name of fromDateTimeString. Consider to use Constants.IANATimeZones. If null will set to LOCAL.</param>
        /// <param name="toTimeZoneIANA">The IANA time zone name of toZonedDateTime and toDateTimeString. Consider to use Constants.IANATimeZones. If null will set to LOCAL.</param>
        /// <param name="toPattern">The format/pattern of toDateTimeString. Consider to use Constants.DateTimePatterns.NodaTime.</param>
        /// <param name="toDateTimeOffset">The output of conversion, in type of DateTimeOffset?.</param>
        /// <param name="toDateTimeString">The output of conversion, in type of string with format/pattern of toPattern.</param>
        /// <param name="cultureInfo">CultureInfo to be used. If null will set to InvariantCulture.</param>
        private static void ConvertTimeZone(DateTime? fromDateTime, string fromTimeZoneIANA, string toPattern, string toTimeZoneIANA,
            out ZonedDateTime? toZonedDateTime, out DateTimeOffset? toDateTimeOffset, out string toDateTimeString,
            CultureInfo cultureInfo = null)
        {
            toZonedDateTime = null;
            toDateTimeOffset = null;
            toDateTimeString = null;
            if (fromDateTime == null) { return; }

            var fromLocalDateTime = LocalDateTime.FromDateTime(fromDateTime.Value);
            var fromTimeZone = fromTimeZoneIANA == null || fromDateTime.Value.Kind == DateTimeKind.Local ? DateTimeZoneProviders.Bcl.GetSystemDefault()
                : DateTimeZoneProviders.Tzdb[fromTimeZoneIANA];
            var toTimeZone = toTimeZoneIANA == null ? DateTimeZoneProviders.Bcl.GetSystemDefault()
                : DateTimeZoneProviders.Tzdb[toTimeZoneIANA];
            ConvertTimeZone(fromLocalDateTime, fromTimeZone, toPattern, toTimeZone, out toZonedDateTime, out toDateTimeOffset, out toDateTimeString, cultureInfo);
        }

        private static void ConvertTimeZone(LocalDateTime? fromLocalDateTime, DateTimeZone fromTimeZone, string toPattern, DateTimeZone toTimeZone,
            out ZonedDateTime? toZonedDateTime, out DateTimeOffset? toDateTimeOffset, out string toDateTimeString,
            CultureInfo cultureInfo = null)
        {
            toZonedDateTime = null;
            toDateTimeOffset = null;
            toDateTimeString = null;
            cultureInfo = cultureInfo ?? CultureInfo.CurrentCulture;
            toPattern = toPattern ?? Constants.DateTime.DateTimePatterns.DateTime;

            if (fromLocalDateTime.HasValue)
            {
                var fromZonedDateTime = fromLocalDateTime.Value.InZoneLeniently(fromTimeZone);
                toZonedDateTime = fromZonedDateTime.WithZone(toTimeZone);
                toDateTimeOffset = toZonedDateTime.Value.ToDateTimeOffset();
                toDateTimeString = toZonedDateTime.Value.ToString(toPattern, cultureInfo);
            }
        } 
        #endregion Helper Methods
    }
}
