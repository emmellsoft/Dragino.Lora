using System;
using Dragino.Gps.Messages.Standard;

namespace Dragino.Gps
{
    /// <summary>
    /// Event arguments holding a <see cref="StandardGpsMessage"/> and the system timestamp of when the message was received.
    /// </summary>
    public class StandardGpsMessageEventArgs : EventArgs
    {
        internal StandardGpsMessageEventArgs(StandardGpsMessage message, DateTime systemReceivedUtcDateTime)
        {
            Message = message;
            SystemReceivedUtcDateTime = systemReceivedUtcDateTime;
        }

        /// <summary>
        /// The standard message received. Check its <see cref="StandardGpsMessage.Kind"/> property to determine what kind of message it is, and cast it accordingly.
        /// </summary>
        public StandardGpsMessage Message { get; }

        /// <summary>
        /// The system timestamp when the message was received.
        /// </summary>
        public DateTime SystemReceivedUtcDateTime { get; }
    }
}