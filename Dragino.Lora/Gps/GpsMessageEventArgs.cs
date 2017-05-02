using System;
using Dragino.Gps.Messages;

namespace Dragino.Gps
{
    /// <summary>
    /// Event arguments holding a <see cref="GpsMessage"/> and the system timestamp of when the message was received.
    /// </summary>
    public class GpsMessageEventArgs : EventArgs
    {
        internal GpsMessageEventArgs(GpsMessage message, DateTime systemReceivedUtcDateTime)
        {
            Message = message;
            SystemReceivedUtcDateTime = systemReceivedUtcDateTime;
        }

        /// <summary>
        /// The standard message received.
        /// </summary>
        public GpsMessage Message { get; }

        /// <summary>
        /// The system timestamp when the message was received.
        /// </summary>
        public DateTime SystemReceivedUtcDateTime { get; }
    }
}