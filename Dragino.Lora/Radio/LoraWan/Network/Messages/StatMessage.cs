using System;

namespace Dragino.Radio.LoraWan.Network.Messages
{
    public class StatMessage
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="time">UTC system time of the gateway.  The precision is one second.  The format is ISO 8601 [3] 'expanded' format.</param>
        /// <param name="latitude">Containing up to 5 decimal places. The latitude of the gateway's position in units of degrees North of the equator.</param>
        /// <param name="longitude">Containing up to 5 decimal places. The longitude of the gateway's position in units of degrees East of the prime meridian.</param>
        /// <param name="altitude">The altitude of the gateway's position in units of metres above sea level (as defined by the United States' GPS system).</param>
        /// <param name="rxnb">The number of radio frames received since gateway start.</param>
        /// <param name="rxok">The number of radio frames received with correct CRC since gateway start.</param>
        /// <param name="rxfw">The number of radio frames forwarded to the gateway's network server since gateway start.</param>
        /// <param name="ackr">The proportion of radio frames that were forwarded to the gateway's network server and acknowledged by the server since gateway start.  The proportion is expressed as a percentage.</param>
        /// <param name="dwnb">The number of radio frames received (from the network server) for transmission since gateway start.</param>
        /// <param name="txnb">The number of radio frames transmitted since gateway start.</param>
        public StatMessage(
            DateTime time,
            double latitude,
            double longitude,
            int altitude,
            uint rxnb,
            uint rxok,
            uint rxfw,
            uint ackr,
            uint dwnb,
            uint txnb)
        {
            Time = time;
            Latitude = latitude;
            Longitude = longitude;
            Altitude = altitude;
            Rxnb = rxnb;
            Rxok = rxok;
            Rxfw = rxfw;
            Ackr = ackr;
            Dwnb = dwnb;
            Txnb = txnb;
        }

        /// <summary>
        /// UTC system time of the gateway.  The precision is one second.  The format is ISO 8601 [3] 'expanded' format.
        /// </summary>
        public DateTime Time { get; }

        /// <summary>
        /// Containing up to 5 decimal places. The latitude of the gateway's position in units of degrees North of the equator.
        /// </summary>
        public double Latitude { get; }

        /// <summary>
        /// Containing up to 5 decimal places. The longitude of the gateway's position in units of degrees East of the prime meridian.
        /// </summary>
        public double Longitude { get; }

        /// <summary>
        /// The altitude of the gateway's position in units of metres above sea level (as defined by the United States' GPS system).
        /// </summary>
        public int Altitude { get; }

        /// <summary>
        /// The number of radio frames received since gateway start.
        /// </summary>
        public uint Rxnb { get; }

        /// <summary>
        /// The number of radio frames received with correct CRC since gateway start.
        /// </summary>
        public uint Rxok { get; }

        /// <summary>
        /// The number of radio frames forwarded to the gateway's network server since gateway start.
        /// </summary>
        public uint Rxfw { get; }

        /// <summary>
        /// The proportion of radio frames that were forwarded to the gateway's network server and acknowledged by the server since gateway start.  The proportion is expressed as a percentage.
        /// </summary>
        public uint Ackr { get; }

        /// <summary>
        /// The number of radio frames received (from the network server) for transmission since gateway start.
        /// </summary>
        public uint Dwnb { get; }

        /// <summary>
        /// The number of radio frames transmitted since gateway start.
        /// </summary>
        public uint Txnb { get; }
    }
}