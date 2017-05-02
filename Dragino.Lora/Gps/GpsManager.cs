using System;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using Dragino.Gps.Messages;
using Dragino.Gps.Messages.Standard;

namespace Dragino.Gps
{
    /// <summary>
    /// High level GPS manager to simplify usage.
    /// </summary>
    internal class GpsManager : IGpsManager
    {
        private readonly SerialDevice _serialDevice;
        private readonly GpsMessageHandler _gpsMessageHandler;
        private bool _isStandby;
        private SimplePosition _currentPosition = SimplePosition.Empty;

        public static async Task<IGpsManager> Create(GpsManagerSettings settings)
        {
            string deviceSelector = SerialDevice.GetDeviceSelector(settings.PortName);
            DeviceInformationCollection deviceInformationCollection = await DeviceInformation.FindAllAsync(deviceSelector);
            SerialDevice serialDevice = await SerialDevice.FromIdAsync(deviceInformationCollection[0].Id);

            if (serialDevice == null)
            {
                throw new Exception(
                    "Serial port not opened. Is the DeviceCapability activated in the Package.appxmanifest file of the project?\r\n" +
                    "   <Capabilities>\r\n" +
                    "     <DeviceCapability Name=\"serialcommunication\">\r\n" +
                    "       <Device Id=\"any\" >\r\n" +
                    "         <Function Type=\"name:serialPort\" />\r\n" +
                    "       </Device>\r\n" +
                    "     </DeviceCapability>\r\n" +
                    "   </Capabilities>");
            }

            return new GpsManager(serialDevice);
        }

        private GpsManager(SerialDevice serialDevice)
        {
            _serialDevice = serialDevice;
            _serialDevice.WriteTimeout = TimeSpan.FromSeconds(1);
            _serialDevice.ReadTimeout = TimeSpan.FromSeconds(1);
            _serialDevice.BaudRate = 9600;
            _serialDevice.Parity = SerialParity.None;
            _serialDevice.StopBits = SerialStopBitCount.One;
            _serialDevice.DataBits = 8;

            _gpsMessageHandler = new GpsMessageHandler(_serialDevice);
            _gpsMessageHandler.OnCustomMessage += HandleCustomMessage;
            _gpsMessageHandler.OnStandardMessage += HandleStandardMessage;

            LatestPosition = new PositionData(AdjustedUtcNow, null, null, GgaFixStatus.Unknown, null, null, null, null, null, null);
            LatestMovement = new MovementData(AdjustedUtcNow, null, null, null, GpsPositioningMode.Unknown);
        }

        public void Dispose()
        {
            _gpsMessageHandler.Dispose();
            _serialDevice.Dispose();
        }

        /// <summary>
        /// Event fired when a standard GPS message arrives.
        /// </summary>
        public event EventHandler<StandardGpsMessageEventArgs> OnStandardMessage;

        /// <summary>
        /// Event fired when a custom GPS message arrives.
        /// </summary>
        public event EventHandler<GpsMessageEventArgs> OnCustomMessage;

        /// <summary>
        /// Event fired when the <see cref="LatestPosition"/> changes.
        /// </summary>
        public event EventHandler<PositionDataEventArgs> OnPositionData;

        /// <summary>
        /// Event fired when the <see cref="LatestMovement"/> changes.
        /// </summary>
        public event EventHandler<MovementDataEventArgs> OnMovementData;

        /// <summary>
        /// The difference between the system clock and the GPS timestamp.
        /// A positive value means that the system clock > GPS clock.
        /// </summary>
        public TimeSpan? UtcDateTimeDifference { get; private set; }

        /// <summary>
        /// Current UTC date time, adjusted to the latest GPS timestamp.
        /// </summary>
        public DateTime AdjustedUtcNow => AdjustSystemUtc(DateTime.UtcNow);

        /// <summary>
        /// Last known position information.
        /// </summary>
        public PositionData LatestPosition { get; private set; }

        /// <summary>
        /// Last known movement information.
        /// </summary>
        public MovementData LatestMovement { get; private set; }

        private void HandleCustomMessage(object sender, GpsMessageEventArgs e)
        {
            OnCustomMessage?.Invoke(this, e);
        }

        private void HandleStandardMessage(object sender, StandardGpsMessageEventArgs e)
        {
            switch (e.Message.Kind)
            {
                case StandardGpsMessageKind.Rmc:
                    HandleRmc((RmcStandardGpsMessage)e.Message);
                    break;
                case StandardGpsMessageKind.Vtg:
                    HandleVtg((VtgStandardGpsMessage)e.Message, e.SystemReceivedUtcDateTime);
                    break;
                case StandardGpsMessageKind.Gga:
                    HandleGga((GgaStandardGpsMessage)e.Message, e.SystemReceivedUtcDateTime);
                    break;
                case StandardGpsMessageKind.Gsa:
                    break;
                case StandardGpsMessageKind.Gsv:
                    break;
                case StandardGpsMessageKind.Gll:
                    break;
                case StandardGpsMessageKind.Txt:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            OnStandardMessage?.Invoke(this, e);
        }

        private void HandleRmc(RmcStandardGpsMessage message)
        {
            if (message.UtcDateTime.HasValue)
            {
                UtcDateTimeDifference = DateTime.UtcNow - message.UtcDateTime.Value;
            }
        }

        private void HandleVtg(VtgStandardGpsMessage message, DateTime systemReceivedUtcDateTime)
        {
            LatestMovement = new MovementData(
                AdjustSystemUtc(systemReceivedUtcDateTime),
                message.CourseOverGround,
                message.SpeedInKnots,
                message.SpeedInKmPerH,
                message.PositioningMode);

            OnMovementData?.Invoke(this, new MovementDataEventArgs(LatestMovement));
        }

        private void HandleGga(GgaStandardGpsMessage message, DateTime systemReceivedUtcDateTime)
        {
            LatestPosition = new PositionData(
                AdjustSystemUtc(systemReceivedUtcDateTime),
                message.Latitude,
                message.Longitude,
                message.FixStatus,
                message.NumberOfSatellites,
                message.HorizontalDilutioOfPrecision,
                message.Altitude,
                message.GeoIdSeparation,
                message.DgpsAge,
                message.DgpsStateId);

            if (LatestPosition.Latitude.HasValue &&
                LatestPosition.Longitude.HasValue &&
                LatestPosition.Altitude.HasValue)
            {
                _currentPosition = new SimplePosition(
                    LatestPosition.Latitude.Value,
                    LatestPosition.Longitude.Value,
                    LatestPosition.Altitude.Value);
            }

            OnPositionData?.Invoke(this, new PositionDataEventArgs(LatestPosition));
        }

        private DateTime AdjustSystemUtc(DateTime systemUtcDateTime)
        {
            return systemUtcDateTime - UtcDateTimeDifference ?? DateTime.UtcNow;
        }

        /// <summary>
        /// Send a message to the GPS chip.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        /// <returns></returns>
        public Task SendMessage(GpsMessage message)
        {
            return _gpsMessageHandler.SendMessage(message);
        }

        /// <summary>
        /// Put the GPS in stand-by mode.
        /// </summary>
        public async Task Standby()
        {
            if (_isStandby)
            {
                return;
            }

            await _gpsMessageHandler.SendMessage(PmtkMessages.Standby).ConfigureAwait(false);
            _isStandby = true;
        }

        /// <summary>
        /// Wake up the GPS from stand-by mode.
        /// </summary>
        public async Task WakeUp()
        {
            if (!_isStandby)
            {
                return;
            }

            await _gpsMessageHandler.SendEmptyMessage().ConfigureAwait(false);
            _isStandby = false;
        }

        SimplePosition IPositionProvider.Position => _currentPosition;
    }
}
