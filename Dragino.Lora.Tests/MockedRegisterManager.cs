using System.Collections.Generic;
using Dragino.Radio;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace Dragino.Lora.Tests
{
	public class MockedRegisterManager
	{
		private readonly Dictionary<byte, byte> _registerValues = new Dictionary<byte, byte>(); // Key: register address, Value: register values
		private byte? _spiOutBuffer;

		public MockedRegisterManager()
		{
			var spiDevice = new MockSpiDevice();
			var mockSpiComm = new MockSpiComm(spiDevice);
			RegisterManager = new RegisterManager(mockSpiComm);

			spiDevice.WriteAction = buffer =>
			{
				bool isWriting = (buffer[0] & 0x80) == 0x80;
				byte address = (byte)(buffer[0] & 0x7F);
				if (isWriting)
				{
					if (buffer.Length != 2)
					{
						Assert.Fail($"Expecting 2 bytes when writing register, got {buffer.Length}.");
					}

					_registerValues[address] = buffer[1];
				}
				else
				{
					if (buffer.Length != 1)
					{
						Assert.Fail($"Expecting 1 byte when reading register, got {buffer.Length}.");
					}

					byte value;
					if (!_registerValues.TryGetValue(address, out value))
					{
						Assert.Fail($"Failed to read uninitialized register byte at address 0x{address:X2}.");
					}

					_spiOutBuffer = value;
				}
			};

			spiDevice.ReadAction = buffer =>
			{
				if (_spiOutBuffer == null)
				{
					Assert.Fail("SPI reading not initialized.");
				}
				else
				{
					buffer[0] = _spiOutBuffer.Value;
					_spiOutBuffer = null;
				}
			};
		}

		public RegisterManager RegisterManager { get; }

		public void ArrangeRegisterValues<T>(params byte[] values) where T : Register
		{
			Register defaultRegister = RegisterSupport.GetDefaultRegister<T>();
			if (defaultRegister.Length != values.Length)
			{
				Assert.Fail($"Failed arranging the register {nameof(T)}; required byte count: {defaultRegister.Length}, given byte count: {values.Length}.");
			}

			for (int i = 0; i < defaultRegister.Length; i++)
			{
				_registerValues[(byte)(defaultRegister.Address + i)] = values[i];
			}
		}

		public byte[] GetRegisterValues<T>() where T : Register
		{
			Register defaultRegister = RegisterSupport.GetDefaultRegister<T>();

			byte[] values = new byte[defaultRegister.Length];

			for (int i = 0; i < defaultRegister.Length; i++)
			{
				byte address = (byte)(defaultRegister.Address + i);

				byte value;
				if (!_registerValues.TryGetValue(address, out value))
				{
					Assert.Fail($"Failed to read uninitialized register byte at address 0x{address:X2}.");
				}

				values[i] = value;
			}

			return values;
		}
	}
}