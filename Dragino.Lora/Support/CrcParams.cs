namespace Dragino.Support
{
    public class CrcParams
    {
        public static readonly CrcParams Ccitt = new CrcParams(0x1D0F, 0x1021, true);
        public static readonly CrcParams Ibm = new CrcParams(0xFFFF, 0x8005, false);

        public CrcParams(ushort seed, ushort polynomial, bool invertResult)
        {
            Seed = seed;
            Polynomial = polynomial;
            InvertResult = invertResult;
        }

        public ushort Seed { get; }

        public ushort Polynomial { get; }

        public bool InvertResult { get; }
    }
}