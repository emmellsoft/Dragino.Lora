namespace Dragino.Radio
{
    public abstract class Register
    {
        protected Register(byte address, byte length)
        {
            Address = address;
            Length = length;
        }

        public byte Address { get; }

        public byte Length { get; }

        internal abstract byte[] GetAsBytes();
    }
}