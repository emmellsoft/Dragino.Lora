namespace Dragino.Gps.Messages.Standard
{
    public class SatelliteInView
    {
        internal SatelliteInView(int satelliteId, int elevation, int azimuth, int snr)
        {
            SatelliteId = satelliteId;
            Elevation = elevation;
            Azimuth = azimuth;
            Snr = snr;
        }

        /// <summary>
        /// Satellite ID.
        /// </summary>
        public int SatelliteId { get; }

        /// <summary>
        /// Elevation in degrees (0-90).
        /// </summary>
        public int Elevation { get; }

        /// <summary>
        /// Azimuth in degrees (0-359).
        /// </summary>
        public int Azimuth { get; }

        /// <summary>
        /// Signal to Noise Ratio in dBHz (0~99), empty if not tracking.
        /// </summary>
        public int Snr { get; }

        public override string ToString()
        {
            return $"ID: \"{SatelliteId}\", Elev: \"{Elevation}\", Azim: \"{Azimuth}\", SNR: \"{Snr}\"";
        }
    }
}