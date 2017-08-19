using System;
using System.Threading;
using System.Threading.Tasks;
using Dragino.Radio;

namespace Dragino.Lora.Fake.Gateway
{
    internal class FakeTransceiver : ITransceiver
    {
        private uint _receivedOkCount;
        private uint _receivedBadCrcCount;
        private uint _receivedTimeoutCount;
        private uint _transmittedOkCount;
        private event EventHandler<ReceivedMessageEventArgs> _messageReceived;

        public event EventHandler<TransmittEventArgs> FakeTransmit;

        public void FakeReceive(ReceivedMessage message)
        {
            if (message.CrcOk && !message.Timeout)
            {
                _receivedOkCount++;
            }
            else
            {
                if (!message.CrcOk)
                {
                    _receivedBadCrcCount++;
                }

                if (message.Timeout)
                {
                    _receivedTimeoutCount++;
                }
            }

            _messageReceived?.Invoke(this, new ReceivedMessageEventArgs(message));
        }

        void IDisposable.Dispose()
        {
        }

        int ITransceiver.MaxTransmitLength => 255;

        uint ITransceiver.ReceivedOkCount => _receivedOkCount;

        uint ITransceiver.ReceivedBadCrcCount => _receivedBadCrcCount;

        uint ITransceiver.ReceivedTimeoutCount => _receivedTimeoutCount;

        uint ITransceiver.TransmittedOkCount => _transmittedOkCount;

        Task<bool> ITransceiver.Transmit(byte[] buffer)
        {
            return ((ITransceiver)this).Transmit(buffer, CancellationToken.None);
        }

        Task<bool> ITransceiver.Transmit(byte[] buffer, CancellationToken ct)
        {
            var transmittEventArgs = new TransmittEventArgs(buffer);
            FakeTransmit?.Invoke(this, transmittEventArgs);
            if (transmittEventArgs.Success)
            {
                _transmittedOkCount++;
            }

            return Task.FromResult(transmittEventArgs.Success);
        }

        event EventHandler<ReceivedMessageEventArgs> ITransceiver.OnMessageReceived
        {
            add { _messageReceived += value; }
            remove { _messageReceived -= value; }
        }
    }
}