using System;
using System.Linq;

namespace Dragino.Radio.LoraWan
{
    /// <summary>
    /// The EUI of the gateway.
    /// </summary>
    public class GatewayEui
    {
        /// <summary>
        /// Create a <see cref="GatewayEui"/> from a byte array.
        /// </summary>
        /// <param name="bytes">Exaxtly 8 bytes.</param>
        public GatewayEui(byte[] bytes)
        {
            if (bytes?.Length != 8)
            {
                throw new ArgumentException("The byte array must be exactly 8 bytes long.");
            }

            Bytes = bytes;
        }

        /// <summary>
        /// Create a <see cref="GatewayEui"/> from a set of hexadecimal characters.
        /// </summary>
        /// <param name="hex">Exactly 16 hexadecimal characters.</param>
        /// <returns></returns>
        public GatewayEui(string hex)
            : this(HexToBytes(hex))
        {
        }

        public byte[] Bytes { get; }

        private static byte[] HexToBytes(string hex)
        {
            if (hex?.Length != 16)
            {
                throw new ArgumentException("The hex string should have exactly 16 characters.");
            }

            return Enumerable
                .Range(0, hex.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                .ToArray();
        }

        public override string ToString() => string.Concat(Bytes.Select(x => x.ToString("X2")));
    }
}