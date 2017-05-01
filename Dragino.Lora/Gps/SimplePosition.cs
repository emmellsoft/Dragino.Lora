namespace Dragino.Gps
{
    public class SimplePosition
    {
        public static readonly SimplePosition Empty = new SimplePosition(0, 0, 0);

        public SimplePosition(double latitude, double longitude, double altitude)
        {
            Latitude = latitude;
            Longitude = longitude;
            Altitude = altitude;
        }

        /// <summary>
        /// The latitude. A negative value means south.
        /// </summary>
        public double Latitude { get; }

        /// <summary>
        /// The longitude. A negative value means west.
        /// </summary>
        public double Longitude { get; }

        /// <summary>
        /// Altitude in meters according to WGS84 ellipsoid.
        /// </summary>
        public double Altitude { get; }
    }
}