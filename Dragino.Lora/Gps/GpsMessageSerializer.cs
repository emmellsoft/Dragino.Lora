using System;
using System.Globalization;
using System.Linq;
using System.Text;
using Dragino.Gps.Messages;

namespace Dragino.Gps
{
    internal static class GpsMessageSerializer
    {
        public static byte[] Serialize(GpsMessage message)
        {
            string messageText = message.MessageId + "," + string.Join(",", message.DataFields);

            byte[] messageBytes = Encoding.ASCII.GetBytes(messageText);
            byte checksumByte = messageBytes.Aggregate<byte, byte>(0, (current, messageByte) => (byte)(current ^ messageByte));
            string checkSum = checksumByte.ToString("X2");

            var packetBytes = new byte[messageBytes.Length + 6];
            packetBytes[0] = (byte)'$';
            Array.Copy(messageBytes, 0, packetBytes, 1, messageBytes.Length);
            packetBytes[messageBytes.Length + 1] = (byte)'*';
            packetBytes[messageBytes.Length + 2] = (byte)checkSum[0];
            packetBytes[messageBytes.Length + 3] = (byte)checkSum[1];
            packetBytes[messageBytes.Length + 4] = (byte)'\r';
            packetBytes[messageBytes.Length + 5] = (byte)'\n';

            return packetBytes;
        }

        public static bool TryDeserialize(string packetText, out GpsMessage message)
        {
            if (packetText == null)
            {
                message = null;
                return false;
            }

            packetText = packetText.Trim();
            int preambleIndex = packetText.LastIndexOf('$');
            if (preambleIndex > 0)
            {
                packetText = packetText.Substring(preambleIndex);
            }

            if (string.IsNullOrEmpty(packetText) ||
                (packetText.Length < 4) ||
                (packetText[0] != '$') ||
                (packetText[packetText.Length - 3] != '*'))
            {
                message = null;
                return false;
            }

            string checkSumText = packetText.Substring(packetText.Length - 2, 2);
            byte checkSum;
            if (!byte.TryParse(checkSumText, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out checkSum))
            {
                message = null;
                return false;
            }

            string messageText = packetText.Substring(1, packetText.Length - 4);
            byte actualCheckSumByte = messageText.Aggregate<char, byte>(0, (current, t) => (byte)(current ^ (byte)t));

            if (actualCheckSumByte != checkSum)
            {
                message = null;
                return false;
            }

            string[] fields = messageText.Split(',');
            message = new GpsMessage(fields[0], fields.Skip(1).ToArray());
            return true;
        }
    }
}