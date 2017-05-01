namespace Dragino.Radio
{
    public class TransceiverSettings
    {
        public static class Standard
        {
            public static readonly TransceiverSettings Europe868 = new TransceiverSettings(
                RadioModemKind.Lora,
                868100000,
                BandWidth.BandWidth_125_00_kHz,
                SpreadingFactor.SF7,
                CodingRate.FourOfFive,
                8,
                true,
                false,
                LoraSyncWord.Public);

            public static readonly TransceiverSettings Europe433 = new TransceiverSettings(
                RadioModemKind.Lora,
                433000000,
                BandWidth.BandWidth_125_00_kHz,
                SpreadingFactor.SF7,
                CodingRate.FourOfFive,
                8,
                true,
                false,
                LoraSyncWord.Public);

            public static readonly TransceiverSettings UnitedStates = new TransceiverSettings(
                RadioModemKind.Lora,
                915000000,
                BandWidth.BandWidth_125_00_kHz,
                SpreadingFactor.SF7,
                CodingRate.FourOfFive,
                8,
                true,
                false,
                LoraSyncWord.Public);
        }

        public TransceiverSettings(
            RadioModemKind radioModem,
            uint frequency,
            BandWidth bandWidth,
            SpreadingFactor spreadingFactor,
            CodingRate codingRate,
            ushort symbolTimeout,
            bool crcEnabled,
            bool enableLowDataRateOptimize,
            LoraSyncWord loraSyncWord)
        {
            RadioModem = radioModem;
            Frequency = frequency;
            BandWidth = bandWidth;
            SpreadingFactor = spreadingFactor;
            CodingRate = codingRate;
            SymbolTimeout = symbolTimeout;
            CrcEnabled = crcEnabled;
            EnableLowDataRateOptimize = enableLowDataRateOptimize;
            LoraSyncWord = loraSyncWord;
        }

        public RadioModemKind RadioModem { get; }

        public uint Frequency { get; }

        public BandWidth BandWidth { get; }

        public SpreadingFactor SpreadingFactor { get; }

        public CodingRate CodingRate { get; }

        public ushort SymbolTimeout { get; }

        public bool CrcEnabled { get; }

        public bool EnableLowDataRateOptimize { get; }

        public LoraSyncWord LoraSyncWord { get; }
    }
}