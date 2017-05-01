using System;

namespace Dragino.Gps
{
    public class PositionDataEventArgs : EventArgs
    {
        public PositionDataEventArgs(PositionData position)
        {
            Position = position;
        }

        public PositionData Position { get; }
    }
}