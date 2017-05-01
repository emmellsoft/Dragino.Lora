using System;

namespace Dragino.Gps.Messages.Standard
{
    /// <summary>
    /// Global positioning system fix data, is the essential fix data which provides 3D location and accuracy data. 
    /// </summary>
    public class GgaStandardGpsMessage : StandardGpsMessage
    {
        public GgaStandardGpsMessage(GpsMessage message)
            : base(StandardGpsMessageKind.Gga)
        {
            // Example of data fields:
            // "015540.000", "3150.68378", "N", "11711.93139", "E", "1", "17", "0.6", "0051.6", "M", "0.0", "M", "", ""

            UtcTimeOfDay = GpsValueParser.ParseUtcTime(message.DataFields[0]);
            Latitude = GpsValueParser.ParseLatitude(message.DataFields[1], message.DataFields[2]);
            Longitude = GpsValueParser.ParseLongitude(message.DataFields[3], message.DataFields[4]);
            FixStatus = GpsValueParser.ParseGgaFixStatus(message.DataFields[5]);
            NumberOfSatellites = GpsValueParser.ParseNullableInt(message.DataFields[6]);
            HorizontalDilutioOfPrecision = GpsValueParser.ParseNullableDouble(message.DataFields[7]);
            Altitude = GpsValueParser.ParseNullableDouble(message.DataFields[8]);
            GeoIdSeparation = GpsValueParser.ParseNullableDouble(message.DataFields[10]);
            DgpsAge = GpsValueParser.ParseNullableDouble(message.DataFields[12]);
            DgpsStateId = GpsValueParser.ParseNullableDouble(message.DataFields[13]);
        }

        /// <summary>
        /// Time of day in UTC.
        /// </summary>
        public TimeSpan? UtcTimeOfDay { get; }

        /// <summary>
        /// The latitude. A negative value means south.
        /// </summary>
        public double? Latitude { get; }

        /// <summary>
        /// The longitude. A negative value means west.
        /// </summary>
        public double? Longitude { get; }

        /// <summary>
        /// Fix status.
        /// </summary>
        public GgaFixStatus FixStatus { get; }

        /// <summary>
        /// Number of satellites being used (0~12).
        /// </summary>
        public int? NumberOfSatellites { get; }

        /// <summary>
        /// Horizontal Dilution of Precision.
        /// </summary>
        public double? HorizontalDilutioOfPrecision { get; }

        /// <summary>
        /// Altitude in meters according to WGS84 ellipsoid.
        /// </summary>
        public double? Altitude { get; }

        /// <summary>
        /// Height of GeoID (mean sea level) above WGS84 ellipsoid, meter.
        /// </summary>
        public double? GeoIdSeparation { get; }

        /// <summary>
        /// Age of DGPS data in seconds, empty if DGPS is not used.
        /// </summary>
        public double? DgpsAge { get; }

        /// <summary>
        /// Age of DGPS data in seconds, empty if DGPS is not used.
        /// </summary>
        public double? DgpsStateId { get; }

        public override string ToString()
        {
            return $"{Kind}: Time=\"{UtcTimeOfDay}\", Lat=\"{Latitude}\", Long=\"{Longitude}\", FixStatus=\"{FixStatus}\", #Sat=\"{NumberOfSatellites}\", HDOP=\"{HorizontalDilutioOfPrecision}\", Alt=\"{Altitude}\", GeoIDSep=\"{GeoIdSeparation}\", DgpsAge=\"{DgpsAge}\", DgpsStateId=\"{DgpsStateId}\"";
        }
    }
}