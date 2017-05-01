using System;

namespace Dragino.Gps
{
    public class FixedPositionProvider : IPositionProvider
    {
        public static readonly IPositionProvider NoPositionProvider = new FixedPositionProvider(SimplePosition.Empty);

        public FixedPositionProvider(SimplePosition currentPosition)
        {
            Position = currentPosition;
        }

        public SimplePosition Position { get; }

        void IDisposable.Dispose()
        {
        }
    }
}