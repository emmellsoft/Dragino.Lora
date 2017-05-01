using Windows.Devices.Gpio;

namespace Dragino.Radio
{
    internal class Pins : IPins
    {
        public Pins(GpioPin resetPin, GpioPin dio0Pin, GpioPin dio1Pin, GpioPin dio2Pin, GpioPin dio3Pin, GpioPin dio4Pin, GpioPin dio5Pin)
        {
            Reset = resetPin;
            Dio0 = dio0Pin;
            Dio1 = dio1Pin;
            Dio2 = dio2Pin;
            Dio3 = dio3Pin;
            Dio4 = dio4Pin;
            Dio5 = dio5Pin;
        }

        public GpioPin Reset { get; }

        public GpioPin Dio0 { get; }

        public GpioPin Dio1 { get; }

        public GpioPin Dio2 { get; }

        public GpioPin Dio3 { get; }

        public GpioPin Dio4 { get; }

        public GpioPin Dio5 { get; }
    }
}