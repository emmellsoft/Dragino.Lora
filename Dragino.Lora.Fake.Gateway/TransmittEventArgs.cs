using System;

namespace Dragino.Lora.Fake.Gateway
{
    public class TransmittEventArgs : EventArgs
    {
        public TransmittEventArgs(byte[] buffer)
        {
            Buffer = buffer;
        }

        public byte[] Buffer { get; }

        public bool Success { get; set; } = true;
    }
}