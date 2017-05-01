namespace Dragino.Radio
{
    public class TransceiverPinSettings
    {
        public static readonly TransceiverPinSettings DraginoLoraGpsHat = new TransceiverPinSettings(25, 17, 4, 23, 24);

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="chipSelectPinNumber">The chip select (a.k.a. NSS) pin number.</param>
        /// <param name="resetPinNumber">The RESET pin number.</param>
        /// <param name="dio0PinNumber">The DIO0 pin number.</param>
        /// <param name="dio1PinNumber">The optional DIO1 pin number.</param>
        /// <param name="dio2PinNumber">The optional DIO2 pin number.</param>
        /// <param name="dio3PinNumber">The optional DIO3 pin number.</param>
        /// <param name="dio4PinNumber">The optional DIO4 pin number.</param>
        /// <param name="dio5PinNumber">The optional DIO5 pin number.</param>
        public TransceiverPinSettings(
            int chipSelectPinNumber,
            int resetPinNumber,
            int dio0PinNumber,
            int? dio1PinNumber = null,
            int? dio2PinNumber = null,
            int? dio3PinNumber = null,
            int? dio4PinNumber = null,
            int? dio5PinNumber = null)
        {
            ChipSelectPinNumber = chipSelectPinNumber;
            ResetPinNumber = resetPinNumber;
            Dio0PinNumber = dio0PinNumber;
            Dio1PinNumber = dio1PinNumber;
            Dio2PinNumber = dio2PinNumber;
            Dio3PinNumber = dio3PinNumber;
            Dio4PinNumber = dio4PinNumber;
            Dio5PinNumber = dio5PinNumber;
        }

        /// <summary>
        /// The chip select (a.k.a. NSS) pin number.
        /// </summary>
        public int ChipSelectPinNumber { get; }

        /// <summary>
        /// The RESET pin number.
        /// </summary>
        public int ResetPinNumber { get; }

        /// <summary>
        /// The DIO0 pin number.
        /// </summary>
        public int Dio0PinNumber { get; }

        /// <summary>
        /// The optional DIO1 pin number.
        /// </summary>
        public int? Dio1PinNumber { get; }

        /// <summary>
        /// The optional DIO2 pin number.
        /// </summary>
        public int? Dio2PinNumber { get; }

        /// <summary>
        /// The optional DIO3 pin number.
        /// </summary>
        public int? Dio3PinNumber { get; }

        /// <summary>
        /// The optional DIO4 pin number.
        /// </summary>
        public int? Dio4PinNumber { get; }

        /// <summary>
        /// The optional DIO5 pin number.
        /// </summary>
        public int? Dio5PinNumber { get; }
    }
}