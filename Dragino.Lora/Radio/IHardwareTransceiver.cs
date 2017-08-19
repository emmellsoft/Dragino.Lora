using Dragino.Support;

namespace Dragino.Radio
{
    public interface IHardwareTransceiver : ITransceiver
    {
        /// <summary>
        /// The creation settings.
        /// </summary>
        TransceiverSettings Settings { get; }

        /// <summary>
        /// Low-level access to the transmit chip registers.
        /// </summary>
        RegisterManager RegisterManager { get; }

        /// <summary>
        /// Low-level access to the SPI communication.
        /// </summary>
        ISpiComm SpiComm { get; }

        /// <summary>
        /// Low-level access to the GPIO pin.
        /// </summary>
        IPins Pins { get; }
    }
}