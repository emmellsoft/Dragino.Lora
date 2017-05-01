namespace Dragino.Gps
{
    public class GpsManagerSettings
    {
        public static readonly GpsManagerSettings Default = new GpsManagerSettings("UART0");

        public GpsManagerSettings(string portName)
        {
            PortName = portName;
        }

        public string PortName { get; }
    }
}