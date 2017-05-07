using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Dragino.Gps;

namespace Dragino.Lora.Demo.Gps
{
    // *********************************************************************************************
    // For the GPS manager to work the serial communication must be allowed in the application.
    //
    // To do this in a new application you need to open the "Package.appxmanifest" file manually
    // (right-click it in the Solution Explorer and select "View Code" - or - select it and press F7)
    // and add the following section to the <Capabilities> node:
    //
    //    <DeviceCapability Name="serialcommunication">
    //      <Device Id="any" >
    //        <Function Type="name:serialPort" />
    //      </Device>
    //    </DeviceCapability>
    // *********************************************************************************************

    public sealed partial class MainPage : Page
    {
        private IGpsManager _gpsManager;
        private MapIcon _mapIcon;

        public MainPage()
        {
            InitializeComponent();

            WorldMap.ZoomLevel = 10;

            Task.Run(async () => await InitGpsManager()).ConfigureAwait(false);
        }

        private async Task InitGpsManager()
        {
            try
            {
                // Create the GPS manager
                _gpsManager = await GpsManagerFactory.Create(GpsManagerSettings.Default).ConfigureAwait(false);

                // Hook up the events
                _gpsManager.OnStandardMessage += GpsManagerStandardMessage;
                _gpsManager.OnCustomMessage += GpsManagerCustomMessage;
                _gpsManager.OnPositionData += GpsManagerPositionDataAsync;
                _gpsManager.OnMovementData += GpsManagerMovementData;

                // Wake up!
                await _gpsManager.WakeUp().ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                WriteLog("The demo crashed:\r\n" + exception.Message);
                Debugger.Break();
            }
        }

        private void GpsManagerStandardMessage(object sender, StandardGpsMessageEventArgs e)
        {
            WriteLog($"Standard Message: \"{e.Message}\", received at {e.SystemReceivedUtcDateTime}");

            // Suppose you are interested in for instance the RMC messages.
            // Then you need to do something like this:

            //----------------------------------------------------------------------------------------------------
            //if (e.Message.Kind == StandardGpsMessageKind.Rmc)
            //{
            //    RmcStandardGpsMessage rmcStandardGpsMessage = (RmcStandardGpsMessage)e.Message;
            //    WriteLog($"RMC Message arrived. My speed over ground is {rmcStandardGpsMessage.Speed} knots.");
            //}
            //----------------------------------------------------------------------------------------------------

            // ...or...

            //----------------------------------------------------------------------------------------------------
            //RmcStandardGpsMessage rmcStandardGpsMessage = e.Message as RmcStandardGpsMessage;
            //if (rmcStandardGpsMessage != null)
            //{
            //    WriteLog($"RMC Message arrived. My speed over ground is {rmcStandardGpsMessage.Speed} knots.");
            //}
            //----------------------------------------------------------------------------------------------------
        }

        private void GpsManagerCustomMessage(object sender, GpsMessageEventArgs e)
        {
            WriteLog($"Custom Message: \"{e.Message}\", received at {e.SystemReceivedUtcDateTime}");
        }

        private async void GpsManagerPositionDataAsync(object sender, PositionDataEventArgs e)
        {
            WriteLog($"Position: {e.Position}");
            await UpdateMapPoint(e.Position).ConfigureAwait(false);
        }

        private void GpsManagerMovementData(object sender, MovementDataEventArgs e)
        {
            WriteLog($"Movement: {e.Movement}");
        }

        private async Task UpdateMapPoint(PositionData positionData)
        {
            if (positionData == null)
            {
                return;
            }

            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal,
                () =>
                {
                    if (!positionData.Latitude.HasValue ||
                        !positionData.Longitude.HasValue)
                    {
                        WorldMap.MapElements.Clear();
                        _mapIcon = null;
                        return;
                    }

                    var geoposition = new BasicGeoposition
                    {
                        Latitude = positionData.Latitude.Value,
                        Longitude = positionData.Longitude.Value
                    };

                    var geopoint = new Geopoint(geoposition);

                    if (_mapIcon == null)
                    {
                        _mapIcon = new MapIcon
                        {
                            Location = geopoint,
                            NormalizedAnchorPoint = new Point(0.5, 0.5),
                            Title = "You are here!"
                        };

                        WorldMap.MapElements.Add(_mapIcon);

                        WorldMap.Center = geopoint;
                    }
                    else
                    {
                        _mapIcon.Location = geopoint;
                    }
                });
        }

        private void WriteLog(string text)
        {
            // Simply writing to the Output window:
            Debug.WriteLine(text);
        }
    }
}
