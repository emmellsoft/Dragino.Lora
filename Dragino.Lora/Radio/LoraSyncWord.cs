namespace Dragino.Radio
{
    public class LoraSyncWord
    {
        public static readonly LoraSyncWord Private = new LoraSyncWord(0x12);

        public static readonly LoraSyncWord Public = new LoraSyncWord(0x34);

        public LoraSyncWord(byte value)
        {
            Value = value;
        }

        public byte Value { get; }
    }
}