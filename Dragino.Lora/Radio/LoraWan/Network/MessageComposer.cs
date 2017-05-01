using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Data.Json;
using Windows.Storage.Streams;

namespace Dragino.Radio.LoraWan.Network
{
    internal class MessageComposer
    {
        private const int HeaderLength = 12;
        private const byte ProtocolVersion = 1;

        private readonly GatewayEui _gatewayEui;
        private static readonly Random _random = new Random();

        public MessageComposer(GatewayEui gatewayEui)
        {
            _gatewayEui = gatewayEui;
        }

        public IBuffer ComposeMessage(JsonObject data, SendMessageKind messageKind)
        {
            byte[] jsonData = Encoding.ASCII.GetBytes(data.Stringify());

            byte[] message = new byte[HeaderLength + jsonData.Length];

            message[0] = ProtocolVersion;
            message[1] = (byte)_random.Next(256);
            message[2] = (byte)_random.Next(256);
            message[3] = (byte)messageKind;
            Array.Copy(_gatewayEui.Bytes, 0, message, 4, _gatewayEui.Bytes.Length);
            Array.Copy(jsonData, 0, message, HeaderLength, jsonData.Length);

            return message.AsBuffer();
        }
    }
}