using System;
using Dragino.Gps.Messages.Standard;

namespace Dragino.Gps
{
    public class StandardGpsMessageEventArgs : EventArgs
    {
        public StandardGpsMessageEventArgs(StandardGpsMessage message, DateTime systemReceivedUtcDateTime)
        {
            Message = message;
            SystemReceivedUtcDateTime = systemReceivedUtcDateTime;
        }

        public StandardGpsMessage Message { get; }

        public DateTime SystemReceivedUtcDateTime { get; }
    }
}