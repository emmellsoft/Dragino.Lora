using System;
using System.Threading;
using System.Threading.Tasks;

namespace Dragino.Radio
{
    public interface ITransceiver : IDisposable
    {
        /// <summary>
        /// The maximum length of a message to be transmitted.
        /// </summary>
        int MaxTransmitLength { get; }

        /// <summary>
        /// Number of successfully received messages.
        /// </summary>
        uint ReceivedOkCount { get; }

        /// <summary>
        /// Number of CRC errors on received messages.
        /// </summary>
        uint ReceivedBadCrcCount { get; }

        /// <summary>
        /// Number of timed-out received messages.
        /// </summary>
        uint ReceivedTimeoutCount { get; }

        /// <summary>
        /// Number of successfully transmitted messages.
        /// </summary>
        uint TransmittedOkCount { get; }

        /// <summary>
        /// Transmit a message.
        /// </summary>
        /// <param name="buffer">The message buffer to send. The maximum number of bytes is <see cref="MaxTransmitLength"/>.</param>
        Task<bool> Transmit(byte[] buffer);

        /// <summary>
        /// Transmit a message.
        /// </summary>
        /// <param name="buffer">The message buffer to send. The maximum number of bytes is <see cref="MaxTransmitLength"/>.</param>
        /// <param name="ct">Cancellation token.</param>
        Task<bool> Transmit(byte[] buffer, CancellationToken ct);

        /// <summary>
        /// An event fired when a message has successfully been received.
        /// </summary>
        event EventHandler<ReceivedMessageEventArgs> OnMessageReceived;
    }
}