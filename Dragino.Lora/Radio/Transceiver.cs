using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using Windows.Devices.Spi;
using Dragino.Radio.Transceivers;
using Dragino.Support;

namespace Dragino.Radio
{
    internal abstract class Transceiver : ITransceiver
    {
        private readonly SpiCommController _spiCommController;
        private bool _isDisposed;
        private readonly object _transmissionsLock = new object();
        private readonly ManualResetEventSlim _transmissionCompletedEventSlim = new ManualResetEventSlim();
        private readonly Queue<Transmission> _transmissions = new Queue<Transmission>();
        private Task _transmissionsTask;
        private readonly object _registerFifoLock = new object();

        private class Transmission
        {
            public Transmission(
                byte[] buffer,
                CancellationToken cancellationToken,
                TaskCompletionSource<bool> transmissionCompletionSource)
            {
                Buffer = buffer;
                CancellationToken = cancellationToken;
                TransmissionCompletionSource = transmissionCompletionSource;
            }

            public byte[] Buffer { get; }

            public CancellationToken CancellationToken { get; }

            public TaskCompletionSource<bool> TransmissionCompletionSource { get; }
        }

        public static async Task<ITransceiver> Create(SpiDevice spiDevice, TransceiverSettings settings, TransceiverPinSettings pinSettings)
        {
            Transceiver transceiver;
            switch (settings.RadioModem)
            {
                case RadioModemKind.Fsk:
                    throw new NotImplementedException();

                case RadioModemKind.Lora:
                    LoraTransceiver.ValidateSettings(settings);
                    transceiver = new LoraTransceiver(spiDevice, settings, pinSettings);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            await transceiver.Init().ConfigureAwait(false);

            return transceiver;
        }

        protected Transceiver(SpiDevice spiDevice, TransceiverSettings settings, TransceiverPinSettings pinSettings)
        {
            Settings = settings;
            _spiCommController = new SpiCommController(spiDevice);

            SpiComm = _spiCommController.Create(pinSettings.ChipSelectPinNumber);

            Pins = CreatePins(pinSettings);
            Pins.Dio0.ValueChanged += Dio0Pin_ValueChanged;

            RegisterManager = new RegisterManager(SpiComm);
        }

        private static IPins CreatePins(TransceiverPinSettings pinSettings)
        {
            GpioController gpioController = GpioController.GetDefault();

            GpioPin resetPin = gpioController.OpenPin(pinSettings.ResetPinNumber);
            resetPin.SetDriveMode(GpioPinDriveMode.Output);

            GpioPin dio0Pin = gpioController.OpenPin(pinSettings.Dio0PinNumber);
            dio0Pin.SetDriveMode(GpioPinDriveMode.Input);
            
            return new Pins(
                resetPin, 
                dio0Pin,
                OpenOptionalPin(gpioController, pinSettings.Dio1PinNumber),
                OpenOptionalPin(gpioController, pinSettings.Dio2PinNumber),
                OpenOptionalPin(gpioController, pinSettings.Dio3PinNumber),
                OpenOptionalPin(gpioController, pinSettings.Dio4PinNumber),
                OpenOptionalPin(gpioController, pinSettings.Dio5PinNumber));
        }
        

        private static GpioPin OpenOptionalPin(GpioController gpioController, int? pinNumber)
        {
            if (!pinNumber.HasValue)
            {
                return null;
            }

            GpioPin pin = gpioController.OpenPin(pinNumber.Value);
            pin.SetDriveMode(GpioPinDriveMode.Input);
            return pin;
        }

        public TransceiverSettings Settings { get; }

        public RegisterManager RegisterManager { get; }

        public ISpiComm SpiComm { get; }

        public IPins Pins { get; }

        public int MaxTransmitLength { get; } = 0x80;

        public uint ReceivedOkCount { get; private set; }

        public uint ReceivedBadCrcCount { get; private set; }

        public uint ReceivedTimeoutCount { get; private set; }

        public uint TransmittedOkCount { get; private set; }

        public event EventHandler<ReceivedMessageEventArgs> OnMessageReceived;

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            SpiComm?.Dispose();

            _spiCommController?.Dispose();

            Pins.Reset?.Dispose();

            if (Pins.Dio0 != null)
            {
                Pins.Dio0.ValueChanged -= Dio0Pin_ValueChanged;
                Pins.Dio0.Dispose();
            }
            Pins.Dio1?.Dispose();
            Pins.Dio2?.Dispose();
            Pins.Dio3?.Dispose();
            Pins.Dio4?.Dispose();
            Pins.Dio5?.Dispose();

            _transmissionCompletedEventSlim?.Dispose();

            _isDisposed = true;
        }

        protected abstract Task Init();

        /// <summary>
        /// Get the RSSI correction constant.
        /// </summary>
        /// <returns></returns>
        private int GetRssiCorrection()
        {
            // Page 87 in Semtech SX1276 datasheet.
            return RegisterManager.Read<CommonRegisterOpMode>().LowFrequencyModeOn == CommonRegisterOpMode.LowFrequencyModeOnEnum.HighFrequencyMode
                ? -157
                : -164;
        }

        private void Dio0Pin_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs args)
        {
            if (args.Edge != GpioPinEdge.RisingEdge)
            {
                return;
            }

            LoraRegisterIrqFlags.FlagsEnum irqFlags = RegisterManager.Read<LoraRegisterIrqFlags>().Flags;
            if (irqFlags.HasFlag(LoraRegisterIrqFlags.FlagsEnum.TxDone))
            {
                TransmittedOkCount++;
                _transmissionCompletedEventSlim.Set();
            }

            if (irqFlags.HasFlag(LoraRegisterIrqFlags.FlagsEnum.RxDone))
            {
                ReceivedMessage message = GetReceivedMessage();

                OnMessageReceived?.Invoke(this, new ReceivedMessageEventArgs(message));
            }

            // Clear all IRQ flags
            RegisterManager.Write(new LoraRegisterIrqFlags(0xFF));
        }

        private ReceivedMessage GetReceivedMessage()
        {
            DateTime timestamp = DateTime.UtcNow;

            LoraRegisterIrqFlags.FlagsEnum irqFlags = RegisterManager.Read<LoraRegisterIrqFlags>().Flags;

            bool timeout;
            if (irqFlags.HasFlag(LoraRegisterIrqFlags.FlagsEnum.RxTimeout))
            {
                timeout = true;
                ReceivedTimeoutCount++;
            }
            else
            {
                timeout = false;
            }

            bool crcOk;
            if (irqFlags.HasFlag(LoraRegisterIrqFlags.FlagsEnum.PayloadCrcError))
            {
                crcOk = false;
                ReceivedBadCrcCount++;
            }
            else
            {
                crcOk = true;
            }

            if (!timeout && crcOk)
            {
                ReceivedOkCount++;
            }

            lock (_registerFifoLock)
            {
                // Get the FIFO-address of the last received packet
                byte currentFifoAddr = RegisterManager.Read<LoraRegisterFifoRxCurrentAddr>().Addr;

                // Go to that address
                RegisterManager.Write(new LoraRegisterFifoAddrPtr(currentFifoAddr));

                // Get the number of received bytes
                byte receivedCount = RegisterManager.Read<LoraRegisterRxNbBytes>().FifoRxBytesNb;

                // Prepare a buffer to copy to from the FIFO queue
                var buffer = new byte[receivedCount];

                // Copy from the FIFO queue
                for (int i = 0; i < receivedCount; i++)
                {
                    buffer[i] = RegisterManager.Read<CommonRegisterFifo>().Value;
                }

                // Get the RSSI correction value
                int rssiCorrection = GetRssiCorrection();

                // Get the adjusted RSSI value
                int rssi = RegisterManager.Read<LoraRegisterRssiValue>().Rssi + rssiCorrection;

                // Get the adjusted RSSI value of the packet
                int pktRssi = RegisterManager.Read<LoraRegisterPktRssiValue>().PacketRssi + rssiCorrection;

                // Get the packet signal-to-noise ratio
                float pktSnr = (float)Math.Round(RegisterManager.Read<LoraRegisterPktSnrValue>().PacketSnr / 4d, 1);

#if DEBUG
                Debug.WriteLine($"Packet RSSI: {pktRssi}, RSSI: {rssi}, SNR: {pktSnr}, Length: {buffer.Length}");
#endif

                return new ReceivedMessage(buffer, rssi, pktRssi, pktSnr, crcOk, timeout, timestamp);
            }
        }

        public Task<bool> Transmit(byte[] buffer)
        {
            return Transmit(buffer, CancellationToken.None);
        }

        public Task<bool> Transmit(byte[] buffer, CancellationToken ct)
        {
            if ((buffer == null) || (buffer.Length == 0))
            {
                // An empty message can always be sent.
                return Task.FromResult(true);
            }

            if (buffer.Length > MaxTransmitLength)
            {
                throw new ArgumentOutOfRangeException(nameof(buffer), $"The maximum message length is {MaxTransmitLength} bytes. Your message was {buffer.Length} bytes.");
            }

            var transmission = new Transmission(buffer, ct, new TaskCompletionSource<bool>());

            lock (_transmissionsLock)
            {
                // Put the transmission in the queue
                _transmissions.Enqueue(transmission);

                // If there is no ongoing transmission, start a task to handle the queue
                if (_transmissionsTask == null)
                {
                    _transmissionsTask = new Task(HandleTransmissions);
                    _transmissionsTask.Start();
                }
            }

            return transmission.TransmissionCompletionSource.Task;
        }

        private void HandleTransmissions()
        {
            while (true)
            {
                Transmission transmission = null;

                try
                {
                    lock (_transmissionsLock)
                    {
                        if (_transmissions.Count == 0)
                        {
                            // The queue is empty. Set the transmission task to null
                            _transmissionsTask = null;
                            return;
                        }

                        // Pick the first transmission in line for sending
                        transmission = _transmissions.Dequeue();
                    }

                    // Send the transmission
                    HandleTransmission(transmission);
                }
                catch (Exception exception)
                {
                    transmission?.TransmissionCompletionSource.SetException(exception);
                }
            }
        }

        private void HandleTransmission(Transmission transmission)
        {
            try
            {
                lock (_registerFifoLock)
                {
                    // Allow the full FIFO size for transmission
                    RegisterManager.Write(new LoraRegisterFifoTxBaseAddr(0));

                    // Position at the beginning of the FIFO
                    RegisterManager.Write(new LoraRegisterFifoAddrPtr(0));

                    // The message data
                    foreach (byte b in transmission.Buffer)
                    {
                        RegisterManager.Write(new CommonRegisterFifo(b));
                    }

                    // Write the message length
                    RegisterManager.Write(new LoraRegisterPayloadLength((byte)transmission.Buffer.Length));

                    _transmissionCompletedEventSlim.Reset();

                    // Start the transmitter
                    EnterTransmitMode();
                }

                try
                {
                    // Wait for the transmission to finish.
                    _transmissionCompletedEventSlim.Wait(transmission.CancellationToken);
                }
                catch (OperationCanceledException)
                {
                }

                // Was it cancelled?
                transmission.TransmissionCompletionSource.SetResult(!transmission.CancellationToken.IsCancellationRequested);
            }
            catch (Exception exception)
            {
                transmission.TransmissionCompletionSource.SetException(exception);
            }
            finally
            {
                // Go back to receiver-mode.
                EnterReceiveMode();
            }
        }

        private void EnterReceiveMode()
        {
            RegisterManager.Write(new CommonRegisterOpMode(
                CommonRegisterOpMode.LongRangeModeEnum.LoRaMode,
                CommonRegisterOpMode.AccessSharedRegEnum.AccessLoRaRegisterPage,
                CommonRegisterOpMode.LowFrequencyModeOnEnum.HighFrequencyMode,
                CommonRegisterOpMode.DeviceModeEnum.ReceiveContinuous));

            RegisterManager.Write(new CommonRegisterDioMapping1(
                CommonRegisterDioMapping1.Dio0MappingEnum.RxDone,
                CommonRegisterDioMapping1.Dio1MappingEnum.RxTimeout,
                CommonRegisterDioMapping1.Dio2MappingEnum.FhssChangeChannel1,
                CommonRegisterDioMapping1.Dio3MappingEnum.CadDone));
        }

        private void EnterTransmitMode()
        {
            RegisterManager.Write(new CommonRegisterOpMode(
                CommonRegisterOpMode.LongRangeModeEnum.LoRaMode,
                CommonRegisterOpMode.AccessSharedRegEnum.AccessLoRaRegisterPage,
                CommonRegisterOpMode.LowFrequencyModeOnEnum.HighFrequencyMode,
                CommonRegisterOpMode.DeviceModeEnum.Transmit));

            // Interrupt on TxDone
            RegisterManager.Write(new CommonRegisterDioMapping1(
                CommonRegisterDioMapping1.Dio0MappingEnum.TxDone,
                CommonRegisterDioMapping1.Dio1MappingEnum.RxTimeout,
                CommonRegisterDioMapping1.Dio2MappingEnum.FhssChangeChannel1,
                CommonRegisterDioMapping1.Dio3MappingEnum.CadDone));
        }
    }
}