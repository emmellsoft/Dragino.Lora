using System;

namespace Dragino.Gps
{
    /// <summary>
    /// A <see cref="IPositionProvider"/> with a hardcoded position.
    /// May be used when no GPS is available.
    /// </summary>
    public class FixedPositionProvider : IPositionProvider
    {
        /// <summary>
        /// No position at all.
        /// </summary>
        public static readonly IPositionProvider NoPositionProvider = new FixedPositionProvider(SimplePosition.Empty);

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="currentPosition">The fixed position.</param>
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