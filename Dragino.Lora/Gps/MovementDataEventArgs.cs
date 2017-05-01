using System;

namespace Dragino.Gps
{
    public class MovementDataEventArgs : EventArgs
    {
        public MovementDataEventArgs(MovementData movement)
        {
            Movement = movement;
        }

        public MovementData Movement { get; }
    }
}