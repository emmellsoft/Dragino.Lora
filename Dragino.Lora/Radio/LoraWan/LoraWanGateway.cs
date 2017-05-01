using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Networking;
using Windows.Storage.Streams;
using Dragino.Gps;
using Dragino.Radio.LoraWan.Network;
using Dragino.Radio.LoraWan.Network.Messages;

namespace Dragino.Radio.LoraWan
{
    internal class LoraWanGateway : ILoraWanGateway
    {
        private readonly ITransceiver _transceiver;
        private readonly LoraWanGatewaySettings _gatewaySettings;
        private readonly IPositionProvider _positionProvider;
        private readonly LoraNetworkClient[] _loraNetworkClients;
        private readonly MessageComposer _messageComposer;
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private static readonly double _ticksScale = Stopwatch.Frequency / 1000000.0;
        private bool _isDisposed;
        private uint _forwardedFrameCount;
        
        public LoraWanGateway(
            ITransceiver transceiver,
            LoraWanGatewaySettings gatewaySettings,
            GatewayEui gatewayEui,
            IPositionProvider positionProvider)
        {
            _stopwatch.Start();

            _transceiver = transceiver;
            _gatewaySettings = gatewaySettings;
            _positionProvider = positionProvider;
            _loraNetworkClients = gatewaySettings.Hosts
                .Select(x => new LoraNetworkClient(new HostName(x), 1700))
                .ToArray();

            _messageComposer = new MessageComposer(gatewayEui);

            _transceiver.OnMessageReceived += TransceiverOnMessageReceived;
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _transceiver.OnMessageReceived -= TransceiverOnMessageReceived;
            _transceiver.Dispose();

            _positionProvider.Dispose();

            _isDisposed = true;
        }

        public event EventHandler<ReceivedMessageEventArgs> OnMessageReceived;

        public Task SendStatus()
        {
            uint receivedCount = _transceiver.ReceivedOkCount +
                _transceiver.ReceivedBadCrcCount +
                _transceiver.ReceivedTimeoutCount;
            
            Debug.WriteLine($"Packages: {receivedCount} / {_transceiver.ReceivedOkCount} / {_forwardedFrameCount}");

            var statMessage = new StatMessage(
                DateTime.UtcNow,
                _positionProvider.Position.Latitude,
                _positionProvider.Position.Longitude,
                (int)Math.Round(_positionProvider.Position.Altitude),
                receivedCount,
                _transceiver.ReceivedOkCount,
                _forwardedFrameCount,
                100, // TODO!
                0, // TODO!
                0); // TODO!

            JsonObject jsonData = JsonSerializer.ToJson(statMessage);

            return PushData(jsonData);
        }

        private Task PushData(JsonObject jsonData)
        {
            IBuffer buffer = _messageComposer.ComposeMessage(jsonData, SendMessageKind.PushData);

            return Task.WhenAll(_loraNetworkClients.Select(x => x.SendMessage(buffer)));
        }

        private async void TransceiverOnMessageReceived(object sender, ReceivedMessageEventArgs e)
        {
            ReceivedMessage message = e.Message;

            if (!message.CrcOk)
            {
                Debug.WriteLine("Message with bad CRC received");
                return;
            }

            if (message.Timeout)
            {
                Debug.WriteLine("Received message timeout");
                return;
            }

            var rxpkMessage = new RxpkMessage(
                message.UtcTimestamp,
                GetElapsedMicroSeconds(),
                _gatewaySettings.Frequency / 1000000.0,
                0, // TODO!
                0, // TODO!
                1, // TODO!
                "LORA", // TODO!
                $"SF{(int)_gatewaySettings.SpreadingFactor}BW{125}", // TODO!
                "4/5", // TODO!
                message.PacketRssi,
                message.PacketSnr,
                (uint)message.Buffer.Length,
                Convert.ToBase64String(message.Buffer));

            JsonObject jsonData = JsonSerializer.ToJson(rxpkMessage);

            Debug.WriteLine(jsonData);

            await PushData(jsonData).ConfigureAwait(false);

            _forwardedFrameCount++;

            Debug.WriteLine("@-@-@------------------------------------------------------------------------------------");
        }

        /// <summary>
        /// Get the elapsed number of microseconds since startup as an uint.
        /// Due to the limitation of uint the elapsed time will wrap to 0 after 4294967295 microseconds (which is roughly 71 and a half minutes).
        /// </summary>
        private uint GetElapsedMicroSeconds()
        {
            ulong elapsedMicroSeconds = (ulong)(_stopwatch.ElapsedTicks / _ticksScale);
            return unchecked((uint)elapsedMicroSeconds);
        }
    }
}