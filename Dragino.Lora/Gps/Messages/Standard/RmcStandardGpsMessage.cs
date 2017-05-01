using System;

namespace Dragino.Gps.Messages.Standard
{
    /// <summary>
    /// Recommended minimum position data (including position, velocity and time).
    /// </summary>
    public class RmcStandardGpsMessage : StandardGpsMessage
    {
        public RmcStandardGpsMessage(GpsMessage message)
            : base(StandardGpsMessageKind.Rmc)
        {
            // Example of data fields:
            // "013732.000", "A", "3150.7238", "N", "11711.7278", "E", "0.00", "0.00", "220413", "", "", "A"

            UtcDateTime = GpsValueParser.ParseUtcDateTime(message.DataFields[8] + message.DataFields[0]);
            DataValid = message.DataFields[1] == "A";
            Latitude = GpsValueParser.ParseLatitude(message.DataFields[2], message.DataFields[3]);
            Longitude = GpsValueParser.ParseLongitude(message.DataFields[4], message.DataFields[5]);
            Speed = GpsValueParser.ParseNullableDouble(message.DataFields[6]);
            CourseOverGround = GpsValueParser.ParseNullableDouble(message.DataFields[7]);
            PositioningMode = GpsValueParser.ParsePositioningMode(message.DataFields[11]);
        }

        /// <summary>
        /// Date and time in UTC format.
        /// </summary>
        public DateTime? UtcDateTime { get; }

        /// <summary>
        /// Is the data valid?
        /// </summary>
        public bool DataValid { get; }

        /// <summary>
        /// The latitude. A negative value means south.
        /// </summary>
        public double? Latitude { get; }

        /// <summary>
        /// The longitude. A negative value means west.
        /// </summary>
        public double? Longitude { get; }

        /// <summary>
        /// The speed over ground in knots.
        /// </summary>
        public double? Speed { get; }

        /// <summary>
        /// The course over ground in degrees.
        /// </summary>
        public double? CourseOverGround { get; }

        /// <summary>
        /// The positioning mode.
        /// </summary>
        public GpsPositioningMode PositioningMode { get; }

        public override string ToString()
        {
            return $"{Kind}: UTC=\"{UtcDateTime}\", DataValid=\"{DataValid}\", Lat=\"{Latitude}\", Long=\"{Longitude}\", Speed=\"{Speed}\", COG=\"{CourseOverGround}\", PosMode=\"{PositioningMode}\"";
        }
    }
}