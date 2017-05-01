using System;
using System.Globalization;

namespace Dragino.Gps.Messages.Standard
{
    internal static class GpsValueParser
    {
        private static readonly DateTime CutOffDate = new DateTime(2017, 1, 1); // Well, we should not expect a date older than this...

        public static DateTime? ParseUtcDateTime(string dateTimeString)
        {
            DateTime dateTime;
            if (!DateTime.TryParseExact(
                dateTimeString,
                "ddMMyyHHmmss'.'fff",
                CultureInfo.InvariantCulture,
                DateTimeStyles.AdjustToUniversal,
                out dateTime))
            {
                return null;
            }

            if (dateTime < CutOffDate)
            {
                return null;
            }

            return dateTime;
        }

        public static TimeSpan? ParseUtcTime(string timeString)
        {
            TimeSpan time;
            if (!TimeSpan.TryParseExact(
                timeString,
                "hhmmss'.'fff",
                CultureInfo.InvariantCulture,
                TimeSpanStyles.None,
                out time))
            {
                return null;
            }

            return time;
        }

        public static int? ParseNullableInt(string value)
        {
            int number;
            if (int.TryParse(value, out number))
            {
                return number;
            }

            return null;
        }

        public static double? ParseNullableDouble(string value)
        {
            double number;
            if (double.TryParse(value, out number))
            {
                return number;
            }

            return null;
        }

        public static double? ParseLatitude(string latitude, string northOrSouth)
        {
            double? value = ParseNullableDouble(latitude);
            if (value == null)
            {
                return null;
            }

            value = ConvertDegMinSecValueToDecimal(value.Value);

            return northOrSouth == "S" ? -value : value;
        }

        public static double? ParseLongitude(string longitude, string eastOrWest)
        {
            double? value = ParseNullableDouble(longitude);
            if (value == null)
            {
                return null;
            }

            value = ConvertDegMinSecValueToDecimal(value.Value);

            return eastOrWest == "W" ? -value : value;
        }

        private static double? ConvertDegMinSecValueToDecimal(double value)
        {
            int deg = (int)value / 100;
            int min = (int)value % 100;
            double sec = value - deg * 100 - min;

            return Math.Round(deg + (min + sec) / 60.0, 6);
        }

        public static GpsPositioningMode ParsePositioningMode(string value)
        {
            switch (value)
            {
                case "N":
                    return GpsPositioningMode.NoFix;
                case "A":
                    return GpsPositioningMode.AutonomousGnssFix;
                case "D":
                    return GpsPositioningMode.DifferentialGnssFix;
                default:
                    return GpsPositioningMode.Unknown;
            }
        }

        public static GgaFixStatus ParseGgaFixStatus(string value)
        {
            switch (value)
            {
                case "0":
                    return GgaFixStatus.Invalid;
                case "1":
                    return GgaFixStatus.GnssFix;
                case "2":
                    return GgaFixStatus.DgpsFix;
                default:
                    return GgaFixStatus.Unknown;
            }
        }

        public static GsaMode ParseGsaMode(string value)
        {
            switch (value)
            {
                case "M":
                    return GsaMode.Manual;
                case "A":
                    return GsaMode.AutoAllowed;
                default:
                    return GsaMode.Unknown;
            }
        }

        public static GsaFixStatus ParseGsaFixStatus(string value)
        {
            switch (value)
            {
                case "1":
                    return GsaFixStatus.NoFix;
                case "2":
                    return GsaFixStatus.Fix2D;
                case "3":
                    return GsaFixStatus.Fix3D;
                default:
                    return GsaFixStatus.Unknown;
            }
        }

        public static TextMessageSeverity ParseTextMessageSeverity(string value)
        {
            switch (value)
            {
                case "00":
                    return TextMessageSeverity.Error;
                case "01":
                    return TextMessageSeverity.Warning;
                case "02":
                    return TextMessageSeverity.Notice;
                case "07":
                    return TextMessageSeverity.User;
                default:
                    return TextMessageSeverity.Unknown;
            }
        }
    }
}