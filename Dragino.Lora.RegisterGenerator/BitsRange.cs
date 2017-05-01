using System;

namespace Dragino.Lora.RegisterGenerator
{
    internal class BitsRange
    {
        public static readonly BitsRange Empty = new BitsRange();

        public static readonly BitsRange Byte = GetByteRange(0);

        private BitsRange()
        {
            IsEmpty = true;
        }

        public BitsRange(int fromBits, int toBits)
        {
            FromBits = fromBits;
            ToBits = toBits;

            MaxValue = 0;
            for (int i = 0; i < BitCount; i++)
            {
                MaxValue = (MaxValue << 1) | 1;
            }
        }

        public bool IsEmpty { get; }

        public int FromBits { get; }

        public int ToBits { get; }

        public int BitCount => ToBits - FromBits + 1;

        public int NibbleCount => (int)Math.Ceiling(BitCount / 4.0);

        public int ByteCount => (int)Math.Ceiling(BitCount / 8.0);

        public int FirstByte => FromBits / 8;

        public int LastByte => ToBits / 8;

        public uint MaxValue { get; }

        public override string ToString() => $"{FromBits}-{ToBits}";

        public static BitsRange GetByteRange(int byteIndex)
        {
            int factor = byteIndex * 8;
            return new BitsRange(factor, factor + 7);
        }

        public BitsRange Intersect(BitsRange bitsRange)
        {
            // bitsRange   .....------...........
            // this        .............-----....

            // this        .....------...........
            // bitsRange   .............-----....

            // bitsRange   ...---------------....
            // this        ....--------..........

            // this        ...---------------....
            // bitsRange   ....--------..........

            // bitsRange   ........----------....
            // this        ....--------..........

            // bitsRange   ....--------..........
            // this        ........----------....

            int from = Math.Max(FromBits, bitsRange.FromBits);
            int to = Math.Min(ToBits, bitsRange.ToBits);

            return from > to
                ? Empty
                : new BitsRange(from, to);
        }

        public byte[] GetByteMasks(int registerLength, int byteIndex)
        {
            byte[] byteMasks = new byte[registerLength];

            int addOn = (registerLength - 1 - byteIndex) * 8;

            BitsRange denormalizedBitsRange = addOn > 0
                ? new BitsRange(FromBits + addOn, ToBits + addOn)
                : this;

            for (int i = 0; i < registerLength; i++)
            {
                BitsRange intersect = denormalizedBitsRange.Intersect(GetByteRange(i));

                intersect = intersect.Shift(-i * 8);

                if (!intersect.IsEmpty)
                {
                    byteMasks[registerLength - i - 1] = (byte)(intersect.MaxValue << intersect.FromBits);
                }
            }

            return byteMasks;
        }

        private BitsRange Shift(int count)
        {
            return IsEmpty ? this : new BitsRange(FromBits + count, ToBits + count);
        }
    }
}