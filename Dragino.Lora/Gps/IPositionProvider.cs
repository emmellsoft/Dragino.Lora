using System;

namespace Dragino.Gps
{
    /// <summary>
    /// Mechanism keeping track of the current position.
    /// </summary>
    public interface IPositionProvider : IDisposable
    {
        SimplePosition Position { get; }
    }
}