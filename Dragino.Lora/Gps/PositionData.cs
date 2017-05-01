using System;
using Dragino.Gps.Messages.Standard;

namespace Dragino.Gps
{
    /// <summary>
    /// Position data.
    /// </summary>
    public class PositionData
    {
        public PositionData(
            DateTime adjustedUtcNow,
            double? latitude,
            double? longitude,
            GgaFixStatus fixStatus,
            int? numberOfSatellites,
            double? horizontalDilutioOfPrecision,
            double? altitude,
            double? geoIdSeparation,
            double? dgpsAge,
            double? dgpsStateId)
        {
            AdjustedUtcNow = adjustedUtcNow;
            Latitude = latitude;
            Longitude = longitude;
            FixStatus = fixStatus;
            NumberOfSatellites = numberOfSatellites;
            HorizontalDilutioOfPrecision = horizontalDilutioOfPrecision;
            Altitude = altitude;
            GeoIdSeparation = geoIdSeparation;
            DgpsAge = dgpsAge;
            DgpsStateId = dgpsStateId;
        }

        /// <summary>
        /// Adjusted UTC date/time.
        /// </summary>
        public DateTime AdjustedUtcNow { get; }

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
            return $"Lat=\"{Latitude}\", Long=\"{Longitude}\", FixStatus=\"{FixStatus}\", #Sat=\"{NumberOfSatellites}\", HDOP=\"{HorizontalDilutioOfPrecision}\", Alt=\"{Altitude}\", GeoIDSep=\"{GeoIdSeparation}\", DgpsAge=\"{DgpsAge}\", DgpsStateId=\"{DgpsStateId}\"";
        }
    }
}