using System.Collections.Generic;
using System.Linq;

namespace Dragino.Gps.Messages.Standard
{
    /// <summary>
    /// GNSS Satellites in View. One GSV sentence can only provide data for at most 4 satellites, so several sentences might be required for the full information.
    /// Since GSV includes satellites that are not used as part of the solution, GSV sentence contains more satellites than GGA does.
    /// </summary>
    public class GsvStandardGpsMessage : StandardGpsMessage
    {
        public GsvStandardGpsMessage(GpsMessage message)
            : base(StandardGpsMessageKind.Gsv)
        {
            // Example of data fields:
            // "3", "1", "12", "01", "05", "060", "18", "02", "17", "259", "43", "04", "56", "287", "28", "09", "08", "277", "28"
            // "3", "2", "12", "10", "34", "195", "46", "13", "08", "125", "45", "17", "67", "014", "", "20", "32", "048", "24"
            // "3", "3", "12", "23", "13", "094", "48", "24", "04", "292", "24", "28", "49", "178", "46", "32", "06", "037", "22"
            // or
            // "1", "1", "00"

            NumberOfMessages = GpsValueParser.ParseNullableInt(message.DataFields[0]) ?? 1;
            SequenceNumber = GpsValueParser.ParseNullableInt(message.DataFields[1]) ?? 1;
            TotalSatellitesInView = GpsValueParser.ParseNullableInt(message.DataFields[2]) ?? 0;

            var satellites = new List<SatelliteInView>();

            int satelliteDataFieldStartIndex = 3;
            while (satelliteDataFieldStartIndex < message.DataFields.Length)
            {
                int satelliteId = GpsValueParser.ParseNullableInt(message.DataFields[satelliteDataFieldStartIndex++]) ?? 0;
                int elevation = GpsValueParser.ParseNullableInt(message.DataFields[satelliteDataFieldStartIndex++]) ?? 0;
                int azimuth = GpsValueParser.ParseNullableInt(message.DataFields[satelliteDataFieldStartIndex++]) ?? 0;
                int snr = GpsValueParser.ParseNullableInt(message.DataFields[satelliteDataFieldStartIndex++]) ?? 0;

                satellites.Add(new SatelliteInView(satelliteId, elevation, azimuth, snr));
            }

            Satellites = satellites.ToArray();
        }

        /// <summary>
        /// Number of messages, total number of GPGSV messages being output (1-3).
        /// </summary>
        public int NumberOfMessages { get; }

        /// <summary>
        /// Sequence number of this entry (1-3).
        /// </summary>
        public int SequenceNumber { get; }

        /// <summary>
        /// Total satellites in view.
        /// </summary>
        public int TotalSatellitesInView { get; }

        /// <summary>
        /// Array of <see cref="SatelliteInView"/>'s.
        /// </summary>
        public SatelliteInView[] Satellites { get; }

        public override string ToString()
        {
            return $"{Kind}: Mess#=\"{NumberOfMessages}\", Seq#=\"{SequenceNumber}\", Sat#=\"{TotalSatellitesInView}\", Sats=[{string.Join(", ", Satellites.Select(x => $"[{x}]"))}]";
        }
    }
}