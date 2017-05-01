using System.Diagnostics;
using Dragino.Support;

namespace Dragino.Radio
{
    public class RegisterManager
    {
        private readonly ISpiComm _spiComm;
        private readonly bool _debugLog;

        public RegisterManager(ISpiComm spiComm, bool debugLog = false)
        {
            _spiComm = spiComm;
            _debugLog = debugLog;
        }

        public T Read<T>() where T : Register
        {
            RegisterConstructorInfo registerConstructorInfo = RegisterConstructorInfo.Get(typeof(T));

            byte[] values = ReadRegister(registerConstructorInfo.Address, registerConstructorInfo.Length);

            return (T)registerConstructorInfo.ConstructorInfo.Invoke(new object[] { values });
        }

        public void Write<T>(T register) where T : Register
        {
            byte[] registerBytes = register.GetAsBytes();
            WriteRegister(register.Address, registerBytes);
        }

        private byte[] ReadRegister(byte registerAddress, byte registerLength)
        {
            byte[] buffer = new byte[registerLength];

            _spiComm.Operate(spiDevice =>
            {
                byte[] oneByte = new byte[1];

                for (int i = 0; i < registerLength; i++)
                {
                    oneByte[0] = (byte)((registerAddress + i) & 0x7F);
                    spiDevice.Write(oneByte);
                    spiDevice.Read(oneByte);
                    buffer[i] = oneByte[0];

                    if (_debugLog)
                    {
                        Debug.WriteLine($"¤¤¤¤ Reading register {oneByte[0]:x2} from {registerAddress + i:x2}");
                    }
                }
            });

            return buffer;
        }

        private void WriteRegister(byte registerAddress, byte[] registerBytes)
        {
            _spiComm.Operate(spiDevice =>
            {
                byte[] twoBytes = new byte[2];

                for (int i = 0; i < registerBytes.Length; i++)
                {
                    twoBytes[0] = (byte)((registerAddress + i) | 0x80);
                    twoBytes[1] = registerBytes[i];

                    if (_debugLog)
                    {
                        Debug.WriteLine($"¤¤¤¤ Writing register {registerBytes[i]:x2} to {registerAddress + i:x2}");
                    }

                    spiDevice.Write(twoBytes);
                }
            });
        }
    }
}