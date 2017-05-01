using System;

namespace Dragino.Gps.Messages.Standard
{
    /// <summary>
    /// Geographic Latitude and Longitude, contains position information, time of position fix and status.
    /// </summary>
    public class GllStandardGpsMessage : StandardGpsMessage
    {
        public GllStandardGpsMessage(GpsMessage message)
            : base(StandardGpsMessageKind.Gll)
        {
            // Example of data fields:
            // "3110.2908", "N", "12123.2348", "E", "041139.000", "A", "A"

            Latitude = GpsValueParser.ParseLatitude(message.DataFields[0], message.DataFields[1]);
            Longitude = GpsValueParser.ParseLongitude(message.DataFields[2], message.DataFields[3]);
            UtcTimeOfDay = GpsValueParser.ParseUtcTime(message.DataFields[4]);
            DataValid = message.DataFields[5] == "A";
            PositioningMode = GpsValueParser.ParsePositioningMode(message.DataFields[6]);
        }

        /// <summary>
        /// The latitude. A negative value means south.
        /// </summary>
        public double? Latitude { get; }

        /// <summary>
        /// The longitude. A negative value means west.
        /// </summary>
        public double? Longitude { get; }

        /// <summary>
        /// Time of day in UTC.
        /// </summary>
        public TimeSpan? UtcTimeOfDay { get; }

        /// <summary>
        /// Is the data valid?
        /// </summary>
        public bool DataValid { get; }

        /// <summary>
        /// The positioning mode.
        /// </summary>
        public GpsPositioningMode PositioningMode { get; }

        public override string ToString()
        {
            return $"{Kind}: UTC=\"{UtcTimeOfDay}\", DataValid=\"{DataValid}\", Lat=\"{Latitude}\", Long=\"{Longitude}\", PosMode=\"{PositioningMode}\"";
        }
    }
}