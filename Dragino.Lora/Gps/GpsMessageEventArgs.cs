using System;
using Dragino.Gps.Messages;

namespace Dragino.Gps
{
    public class GpsMessageEventArgs : EventArgs
    {
        public GpsMessageEventArgs(GpsMessage message, DateTime systemReceivedUtcDateTime)
        {
            Message = message;
            SystemReceivedUtcDateTime = systemReceivedUtcDateTime;
        }

        public GpsMessage Message { get; }

        public DateTime SystemReceivedUtcDateTime { get; }
    }
}