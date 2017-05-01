using System.Linq;

namespace Dragino.Gps.Messages.Standard
{
    /// <summary>
    /// GNSS DOP and Active Satellites, provides details on the fix, including the numbers of the satellites being used and the DOP. At most the first 12 satellite IDs are output.
    /// </summary>
    public class GsaStandardGpsMessage : StandardGpsMessage
    {
        public GsaStandardGpsMessage(GpsMessage message)
            : base(StandardGpsMessageKind.Gsa)
        {
            // Example of data fields:
            // "A", "3", "14", "06", "16", "31", "23", "", "", "", "", "", "", "", "1.66", "1.42", "0.84"

            Mode = GpsValueParser.ParseGsaMode(message.DataFields[0]);
            FixStatus = GpsValueParser.ParseGsaFixStatus(message.DataFields[1]);

            SatelliteUsedPerChannelArray = new int?[12];
            for (int i = 0; i < SatelliteUsedPerChannelArray.Length; i++)
            {
                SatelliteUsedPerChannelArray[i] = GpsValueParser.ParseNullableInt(message.DataFields[2 + i]);
            }

            PositionDilutionOfPrecision = GpsValueParser.ParseNullableDouble(message.DataFields[14]);
            HorizontalDilutionOfPrecision = GpsValueParser.ParseNullableDouble(message.DataFields[15]);
            VerticalDilutionOfPrecision = GpsValueParser.ParseNullableDouble(message.DataFields[16]);
        }

        /// <summary>
        /// Auto selection of 2D or 3D fix.
        /// </summary>
        public GsaMode Mode { get; }

        /// <summary>
        /// Fix status.
        /// </summary>
        public GsaFixStatus FixStatus { get; }

        /// <summary>
        /// The satellite used on the different channels.
        /// Array index 0 holds the satellite used on channel 1, and so on.
        /// Array index 11 holds the satellite used on channel 12.
        /// Always 12 entries.
        /// </summary>
        public int?[] SatelliteUsedPerChannelArray { get; }

        /// <summary>
        /// The Position Dilution of Precision.
        /// </summary>
        public double? PositionDilutionOfPrecision { get; }

        /// <summary>
        /// The Horizontal Dilution of Precision.
        /// </summary>
        public double? HorizontalDilutionOfPrecision { get; }

        /// <summary>
        /// The Vertical Dilution of Precision.
        /// </summary>
        public double? VerticalDilutionOfPrecision { get; }

        public override string ToString()
        {
            return $"{Kind}: Mode=\"{Mode}\", FixStatus=\"{FixStatus}\", Sat=[\"{string.Join(",", SatelliteUsedPerChannelArray.Select(x => $"\"{x}\""))}\"], PDOP=\"{PositionDilutionOfPrecision}\", HDOP=\"{HorizontalDilutionOfPrecision}\", VDOP=\"{VerticalDilutionOfPrecision}\"";
        }
    }
}