using System;

namespace Dragino.Radio
{
    public class RegisterAttribute : Attribute
    {
        public RegisterAttribute(byte address, byte length)
        {
            Address = address;
            Length = length;
        }

        public byte Address { get; }

        public byte Length { get; }
    }
}