using System;

namespace Dragino.Radio
{
    public class ReceivedMessageEventArgs : EventArgs
    {
        public ReceivedMessageEventArgs(ReceivedMessage message)
        {
            Message = message;
        }

        public ReceivedMessage Message { get; }
    }
}