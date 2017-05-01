using System;
using System.Threading.Tasks;
using Windows.Devices.Spi;

namespace Dragino.Radio
{
    public static class TransceiverFactory
    {
        public static async Task<ITransceiver> Create(TransceiverSettings transceiverSettings, TransceiverPinSettings pinSettings)
        {
            SpiDevice spiDevice = await CreateSpiDevice().ConfigureAwait(false);

            return await Transceiver.Create(spiDevice, transceiverSettings, pinSettings).ConfigureAwait(false);
        }

        private static async Task<SpiDevice> CreateSpiDevice()
        {
            SpiController spiController = await SpiController.GetDefaultAsync();

            var settings = new SpiConnectionSettings(0)
            {
                ClockFrequency = 500000,
                Mode = SpiMode.Mode0
            };

            return spiController.GetDevice(settings);
        }
    }
}