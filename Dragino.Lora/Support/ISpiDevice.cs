namespace Dragino.Support
{
    /// <summary>
    /// SPI device interface.
    /// </summary>
    public interface ISpiDevice
    {
        /// <summary>
        /// Writes to the connected device.
        /// </summary>
        /// <param name="buffer">Array containing the data to write to the device.</param>
        void Write(byte[] buffer);

        /// <summary>
        /// Reads from the connected device.
        /// </summary>
        /// <param name="buffer">Array containing data read from the device.</param>
        void Read(byte[] buffer);
    }
}