using System;
using Windows.Devices.Gpio;
using Windows.Devices.Spi;

namespace Dragino.Support
{
    internal class SpiCommController
    {
        private readonly SpiDeviceWrapper _spiDeviceWrapper;
        private readonly object _syncObj = new object();

        private class SpiDeviceWrapper : ISpiDevice, IDisposable
        {
            private readonly SpiDevice _spiDevice;

            public SpiDeviceWrapper(SpiDevice spiDevice)
            {
                _spiDevice = spiDevice;
            }

            public void Dispose()
            {
                _spiDevice.Dispose();
            }

            public void Write(byte[] buffer)
            {
                _spiDevice.Write(buffer);
            }

            public void Read(byte[] buffer)
            {
                _spiDevice.Read(buffer);
            }
        }

        private class SpiComm : ISpiComm
        {
            private readonly ISpiDevice _spiDevice;
            private readonly GpioPin _chipSelectGpioPin;
            private readonly object _syncObj;
            private bool _isDisposed;

            public SpiComm(ISpiDevice spiDevice, GpioPin chipSelectGpioPin, object syncObj)
            {
                _spiDevice = spiDevice;
                _chipSelectGpioPin = chipSelectGpioPin;
                _syncObj = syncObj;

                _chipSelectGpioPin.Write(GpioPinValue.High);
            }

            public void Dispose()
            {
                if (_isDisposed)
                {
                    return;
                }

                _isDisposed = true;
                _chipSelectGpioPin.Dispose();
            }

            public void Operate(Action<ISpiDevice> spiAction)
            {
                Operate(spiDevice => { spiAction(spiDevice); return false; });
            }

            public T Operate<T>(Func<ISpiDevice, T> spiAction)
            {
                try
                {
                    T result;

                    lock (_syncObj)
                    {
                        _chipSelectGpioPin.Write(GpioPinValue.Low);

                        try
                        {
                            result = spiAction(_spiDevice);
                        }
                        finally
                        {
                            _chipSelectGpioPin.Write(GpioPinValue.High);
                        }
                    }

                    return result;
                }
                catch
                {
                    if (_isDisposed)
                    {
                        return default(T);
                    }

                    throw;
                }
            }
        }

        public SpiCommController(SpiDevice spiDevice)
        {
            _spiDeviceWrapper = new SpiDeviceWrapper(spiDevice);
        }

        public void Dispose()
        {
            _spiDeviceWrapper.Dispose();
        }

        public ISpiComm Create(int chipSelectPinNumber)
        {
            GpioPin chipSelectGpioPin = GpioController.GetDefault().OpenPin(chipSelectPinNumber);
            chipSelectGpioPin.SetDriveMode(GpioPinDriveMode.Output);
            return new SpiComm(_spiDeviceWrapper, chipSelectGpioPin, _syncObj);
        }
    }
}