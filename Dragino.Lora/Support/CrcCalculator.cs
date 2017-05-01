namespace Dragino.Support
{
    public class CrcCalculator
    {
        public static ushort CalculateCrc(byte[] buffer, CrcParams crcParams)
        {
            ushort crc = crcParams.Seed;

            for (int byteIndex = 0; byteIndex < buffer.Length; byteIndex++)
            {
                byte data = buffer[byteIndex];
                for (byte bitIndex = 0; bitIndex < 8; bitIndex++)
                {
                    bool match = (((crc & 0x8000) >> 8) ^ (data & 0x80)) != 0;

                    crc <<= 1;

                    if (match)
                    {
                        crc ^= crcParams.Polynomial;
                    }

                    data <<= 1;
                }
            }

            if (crcParams.InvertResult)
            {
                crc = (ushort)~crc;
            }

            return crc;
        }
    }
}