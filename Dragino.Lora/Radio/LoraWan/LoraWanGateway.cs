using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Dragino.Gps;
using Dragino.Radio.LoraWan.Network;
using Dragino.Radio.LoraWan.Network.Messages;

namespace Dragino.Radio.LoraWan
{
    internal class LoraWanGateway : ILoraWanGateway
    {
        private static readonly double _ticksScale = Stopwatch.Frequency / 1000000.0;

        private readonly ITransceiver _transceiver;
        private readonly TransceiverSettings _transceiverSettings;
        private readonly IPositionProvider _positionProvider;
        private readonly ILoraNetworkClient[] _loraNetworkClients;
        private readonly MessageComposer _messageComposer;
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private readonly string _datr;
        private readonly string _codr;
        private bool _isDisposed;
        private uint _forwardedFrameCount;

        public LoraWanGateway(
            ITransceiver transceiver,
            TransceiverSettings transceiverSettings,
            GatewayEui gatewayEui,
            ILoraNetworkClient[] loraNetworkClients,
            IPositionProvider positionProvider)
        {
            _stopwatch.Start();

            _transceiver = transceiver;
            _transceiverSettings = transceiverSettings;
            _positionProvider = positionProvider;
            _loraNetworkClients = loraNetworkClients;

            _messageComposer = new MessageComposer(gatewayEui);

            _transceiver.OnMessageReceived += TransceiverOnMessageReceived;

            _datr = GetSF() + GetBW();
            _codr = GetCodr();
        }

        private string GetSF()
        {
            switch (_transceiverSettings.SpreadingFactor)
            {
                case SpreadingFactor.SF7:
                    return "SF7";
                case SpreadingFactor.SF8:
                    return "SF8";
                case SpreadingFactor.SF9:
                    return "SF9";
                case SpreadingFactor.SF10:
                    return "SF10";
                case SpreadingFactor.SF11:
                    return "SF11";
                case SpreadingFactor.SF12:
                    return "SF12";
                default:
                    throw new ArgumentOutOfRangeException(nameof(_transceiverSettings.SpreadingFactor), "Unsupported spreading factor!");
            }
        }

        private string GetBW()
        {
            switch (_transceiverSettings.BandWidth)
            {
                case BandWidth.BandWidth_125_00_kHz:
                    return "BW125";
                case BandWidth.BandWidth_250_00_kHz:
                    return "BW250";
                case BandWidth.BandWidth_500_00_kHz:
                    return "BW500";
                default:
                    throw new ArgumentOutOfRangeException(nameof(_transceiverSettings.BandWidth), "Unsupported bandwidth!");
            }
        }

        private string GetCodr()
        {
            switch (_transceiverSettings.CodingRate)
            {
                case CodingRate.FourOfFive:
                    return "4/5";
                case CodingRate.FourOfSix:
                    return "4/6";
                case CodingRate.FourOfSeven:
                    return "4/7";
                case CodingRate.FourOfEight:
                    return "4/8";
                default:
                    throw new ArgumentOutOfRangeException(nameof(_transceiverSettings.CodingRate), "Unsupported coding rate!");
            }
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
                100, // Let's make it easy and say everything's fine!
                0,   // This is just a packet-forwarder; no messages will be transmitted.
                0);  // This is just a packet-forwarder; no messages will be transmitted.

            string jsonData = JsonSerializer.ToJson(statMessage);

            Debug.WriteLine("Sending JSON: " + jsonData);

            return PushData(jsonData);
        }

        private Task PushData(string jsonData)
        {
            byte[] buffer = _messageComposer.ComposeMessage(jsonData, NetworkMessageKind.PushData);

            return Task.WhenAll(_loraNetworkClients.Select(x => x.SendMessage(buffer)));
        }

        private async void TransceiverOnMessageReceived(object sender, ReceivedMessageEventArgs e)
        {
            ReceivedMessage message = e.Message;

            Debug.WriteLine("Received: " + message);
            if (!message.CrcOk || message.Timeout)
            {
                return;
            }

            var rxpkMessage = new RxpkMessage(
                message.UtcTimestamp,
                GetElapsedMicroSeconds(),
                _transceiverSettings.Frequency / 1000000.0,
                0,
                0,
                1,
                "LORA",
                _datr,
                _codr,
                message.PacketRssi,
                message.PacketSnr,
                (uint)message.Buffer.Length,
                Convert.ToBase64String(message.Buffer));

            string jsonData = JsonSerializer.ToJson(rxpkMessage);

            Debug.WriteLine("Sending JSON: " + jsonData);

            await PushData(jsonData).ConfigureAwait(false);

            _forwardedFrameCount++;
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