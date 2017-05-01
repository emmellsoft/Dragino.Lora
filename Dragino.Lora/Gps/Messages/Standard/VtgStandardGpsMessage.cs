namespace Dragino.Gps.Messages.Standard
{
    /// <summary>
    /// Track made good and ground speed. 
    /// </summary>
    public class VtgStandardGpsMessage : StandardGpsMessage
    {
        public VtgStandardGpsMessage(GpsMessage message)
            : base(StandardGpsMessageKind.Vtg)
        {
            // Example of data fields:
            // "0.0", "T", "", "M", "0.0", "N", "0.1", "K", "A"

            CourseOverGround = GpsValueParser.ParseNullableDouble(message.DataFields[0]);
            SpeedInKnots = GpsValueParser.ParseNullableDouble(message.DataFields[4]);
            SpeedInKmPerH = GpsValueParser.ParseNullableDouble(message.DataFields[6]);
            PositioningMode = GpsValueParser.ParsePositioningMode(message.DataFields[8]);
        }

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
            return $"{Kind}: COG=\"{CourseOverGround}\", Speed=\"{SpeedInKnots}\" (kn) / \"{SpeedInKmPerH}\" (km/h), PosMode=\"{PositioningMode}\"";
        }
    }
}