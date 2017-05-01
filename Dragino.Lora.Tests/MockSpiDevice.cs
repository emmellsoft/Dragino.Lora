using System;
using Dragino.Radio;
using Dragino.Support;

namespace Dragino.Lora.Tests
{
	public class MockSpiDevice : ISpiDevice
	{
		public int WriteActionCount { get; private set; }

		public int ReadActionCount { get; private set; }

		public Action<byte[]> WriteAction { get; set; }

		public Action<byte[]> ReadAction { get; set; }

		void ISpiDevice.Write(byte[] buffer)
		{
			WriteAction?.Invoke(buffer);
			WriteActionCount++;
		}

		void ISpiDevice.Read(byte[] buffer)
		{
			ReadAction?.Invoke(buffer);
			ReadActionCount++;
		}
	}
}