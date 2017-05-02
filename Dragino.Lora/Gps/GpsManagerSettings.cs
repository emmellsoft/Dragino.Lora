namespace Dragino.Gps
{
    public class GpsManagerSettings
    {
        public static readonly GpsManagerSettings Default = new GpsManagerSettings();

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="portName">The COM port name. Default is "UART0".</param>
        public GpsManagerSettings(string portName = "UART0")
        {
            PortName = portName;
        }

        /// <summary>
        /// The COM port name.
        /// </summary>
        public string PortName { get; }
    }
}