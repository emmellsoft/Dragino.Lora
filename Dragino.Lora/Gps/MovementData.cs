using System;
using Dragino.Gps.Messages.Standard;

namespace Dragino.Gps
{
    /// <summary>
    /// Movement data.
    /// </summary>
    public class MovementData
    {
        public MovementData(
            DateTime adjustedUtcNow,
            double? courseOverGround,
            double? speedInKnots,
            double? speedInKmPerH,
            GpsPositioningMode positioningMode)
        {
            AdjustedUtcNow = adjustedUtcNow;
            CourseOverGround = courseOverGround;
            SpeedInKnots = speedInKnots;
            SpeedInKmPerH = speedInKmPerH;
            PositioningMode = positioningMode;
        }

        /// <summary>
        /// Adjusted UTC date/time.
        /// </summary>
        public DateTime AdjustedUtcNow { get; }

        /// <summary>
        /// Course over ground (true) in degree.
        /// </summary>
        public double? CourseOverGround { get; }

        /// <summary>
        /// Speed over ground in knots.
        /// </summary>
        public double? SpeedInKnots { get; }

        /// <summary>
        /// Speed over ground in km/h.
        /// </summary>
        public double? SpeedInKmPerH { get; }

        /// <summary>
        /// The positioning mode.
        /// </summary>
        public GpsPositioningMode PositioningMode { get; }

        public override string ToString()
        {
            return $"COG=\"{CourseOverGround}\", Speed=\"{SpeedInKnots}\" (kn) / \"{SpeedInKmPerH}\" (km/h), PosMode=\"{PositioningMode}\"";
        }
    }
}