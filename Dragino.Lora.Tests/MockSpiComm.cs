using System;
using Dragino.Radio;
using Dragino.Support;

namespace Dragino.Lora.Tests
{
	public class MockSpiComm : ISpiComm
	{
		private readonly ISpiDevice _spiDevice;

		public MockSpiComm(ISpiDevice spiDevice)
		{
			_spiDevice = spiDevice;
		}

		public void Dispose()
		{
		}

		public void Operate(Action<ISpiDevice> spiAction)
		{
			spiAction(_spiDevice);
		}

		public T Operate<T>(Func<ISpiDevice, T> spiAction)
		{
			return spiAction(_spiDevice);
		}
	}
}