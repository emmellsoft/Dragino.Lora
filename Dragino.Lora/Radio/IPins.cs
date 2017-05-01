using Windows.Devices.Gpio;

namespace Dragino.Radio
{
    public interface IPins
    {
        /// <summary>
        /// Low-level access to the RESET pin.
        /// </summary>
        GpioPin Reset { get; }

        /// <summary>
        /// Low-level access to the DIO0 pin.
        /// </summary>
        GpioPin Dio0 { get; }

        /// <summary>
        /// Low-level access to the DIO1 pin.
        /// Optional - may be null.
        /// </summary>
        GpioPin Dio1 { get; }

        /// <summary>
        /// Low-level access to the DIO2 pin.
        /// Optional - may be null.
        /// </summary>
        GpioPin Dio2 { get; }

        /// <summary>
        /// Low-level access to the DIO3 pin.
        /// Optional - may be null.
        /// </summary>
        GpioPin Dio3 { get; }

        /// <summary>
        /// Low-level access to the DIO4 pin.
        /// Optional - may be null.
        /// </summary>
        GpioPin Dio4 { get; }

        /// <summary>
        /// Low-level access to the DIO5 pin.
        /// Optional - may be null.
        /// </summary>
        GpioPin Dio5 { get; }
    }
}