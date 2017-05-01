using System;

namespace Dragino.Support
{
    /// <summary>
    /// SPI communication helper.
    /// </summary>
    public interface ISpiComm : IDisposable
    {
        /// <summary>
        /// Use the SPI connection.
        /// </summary>
        /// <param name="spiAction">Callback that gives access to the actual <see cref="ISpiDevice"/>.</param>
        void Operate(Action<ISpiDevice> spiAction);

        /// <summary>
        /// Use the SPI connection, returning a value.
        /// </summary>
        /// <param name="spiAction">Callback that gives access to the actual <see cref="ISpiDevice"/>.</param>
        T Operate<T>(Func<ISpiDevice, T> spiAction);
    }
}