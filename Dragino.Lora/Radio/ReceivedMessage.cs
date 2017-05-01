using System;
using System.Linq;

namespace Dragino.Radio
{
    public class ReceivedMessage
    {
        public ReceivedMessage(
            byte[] buffer,
            int rssi,
            int packetRssi,
            float packetSnr,
            bool crcOk,
            bool timeout,
            DateTime utcTimestamp)
        {
            Buffer = buffer;
            Rssi = rssi;
            PacketRssi = packetRssi;
            PacketSnr = packetSnr;
            CrcOk = crcOk;
            Timeout = timeout;
            UtcTimestamp = utcTimestamp;
        }

        /// <summary>
        /// The message buffer received.
        /// </summary>
        public byte[] Buffer { get; }

        /// <summary>
        /// The received signal strength indicator.
        /// </summary>
        public int Rssi { get; }

        /// <summary>
        /// The Received Signal Strength Indicator of the received packet.
        /// </summary>
        public int PacketRssi { get; }

        /// <summary>
        /// The Signal-to-Noise Ratio of the received packet.
        /// </summary>
        public float PacketSnr { get; }

        /// <summary>
        /// Was the CRC OK or not (or unknown)?
        /// </summary>
        public bool CrcOk { get; }

        /// <summary>
        /// Was there a timeout in the reception?
        /// </summary>
        public bool Timeout { get; }

        /// <summary>
        /// Received UTC timestamp.
        /// </summary>
        public DateTime UtcTimestamp { get; }

        public override string ToString()
        {
            return string.Concat(
                CrcOk ? "CRC OK, " : "Bad CRC, ",
                Timeout ? "Timeout, " : "",
                $"Rssi={Rssi}, ",
                $"PacketRssi={PacketRssi}, ",
                $"PacketSnr={PacketSnr}, ",
                $"Buffer:[{string.Join(", ", Buffer.Select(x => x.ToString("x2")))}], ",
                UtcTimestamp.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss"));
        }
    }
}