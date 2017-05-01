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
using Dragino.Radio;
using Dragino.Radio.LoraWan;
using Dragino.Support;

namespace Dragino.Lora.Demo.Gateway
{
    // *********************************************************************************************
    // YOUR EDIT IS REQUIRED HERE!
    //
    // For the GPS manager to work (if you have a GPS chip on your Dragino board),
    // the serial communication must be allowed in the application.
    //
    // To do this in a new application you need to open the "Package.appxmanifest" file manually
    // (right-click it in the Solution Explorer and select "View Code" or select it and press F7)
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
        // *********************************************************************************************
        // YOUR EDIT IS REQUIRED HERE!
        // Choose correct gateway settings:
        private static readonly LoraWanGatewaySettings _gatewaySettings = LoraWanGatewaySettings.TheThingsNetwork.Europe868;
        // *********************************************************************************************

        private MapIcon _mapIcon;

        public MainPage()
        {
            InitializeComponent();

            WorldMap.ZoomLevel = 10;

            Task.Run(async () => await MainLoop()).ConfigureAwait(false);
        }

        private static Task<ITransceiver> CreateTransceiver(LoraWanGatewaySettings settings)
        {
            // *********************************************************************************************
            // YOUR EDIT IS REQUIRED HERE!
            // 
            // Depending on the kind of Dragino expansion board you have, uncomment the right line below!
            // *********************************************************************************************


            // EITHER: I have a Dragino LoRa/GPS HAT attached on my Raspberry Pi:
            return TransceiverFactory.Create(settings, TransceiverPinSettings.DraginoLoraGpsHat);


            // OR: I have a Dragino LoRa (GPS) Arduino Shield connected via wires to the GPIO on my Raspberry Pi:
            //return TransceiverFactory.Create(settings, GetManualPins());  // <-- Edit the GetManualPins-method below!
        }

        /// <summary>
        /// Manual pin settings for a Dragino LoRa (GPS) Arduino Shield connected via wires to the GPIO on the Raspberry Pi.
        /// </summary>
        private static TransceiverPinSettings GetManualPins()
        {
            return new TransceiverPinSettings(
                25,  // ChipSelect
                17,  // Reset
                4,   // Dio0
                23,  // Dio1 (Optional -- you may use null)
                24); // Dio2 (Optional -- you may use null)
        }

        private static Task<IGpsManager> CreateGpsManager()
        {
            // *********************************************************************************************
            // YOUR EDIT IS REQUIRED HERE!
            // 
            // Depending on the kind of Dragino expansion board you have, uncomment the right line below!
            // *********************************************************************************************


            // EITHER: I am using an Dragino extension board having a GPS chip:
            return GpsManagerFactory.Create(GpsManagerSettings.Default);


            // OR: There is no GPS chip on my Dragino extension board:
            //return Task.FromResult<IGpsManager>(null);
        }

        /// <summary>
        /// Get the Gateway EUI.
        /// </summary>
        /// <returns></returns>
        private static Task<GatewayEui> GetGatewayEui()
        {
            // *********************************************************************************************
            // YOUR EDIT IS REQUIRED HERE!
            // 
            // Either use a hardcoded EUI or the MAC address of the device (e.g. Raspberry Pi)!
            // *********************************************************************************************

            // EITHER: Use the MAC address:
            var piUser = new System.Net.NetworkCredential("Administrator", "p@ssw0rd"); // <-- Edit the administrator password for your Raspberry Pi here!
            return MacAddressToGatewayEui.GetGatewayEui(piUser);


            // OR: Use a hard coded EUI:
            //return Task.FromResult(new GatewayEui("b827ebffff39cc04")); // <-- #1
            //return Task.FromResult(new GatewayEui("B827EBFFFF89DD56")); // <-- #2
        }

        private async Task MainLoop()
        {
            try
            {
                // Create the transceiver:
                ITransceiver transceiver = await CreateTransceiver(_gatewaySettings).ConfigureAwait(false);

                // Create the GPS manager (if existing):
                IPositionProvider positionProvider;
                IGpsManager gpsManager = await CreateGpsManager().ConfigureAwait(false);
                if (gpsManager != null)
                {
                    gpsManager.OnPositionData += GpsManagerPositionDataAsync;
                    await gpsManager.WakeUp().ConfigureAwait(false);
                    positionProvider = gpsManager;
                }
                else
                {
                    positionProvider = FixedPositionProvider.NoPositionProvider;
                }

                // Get the gateway EUI:
                GatewayEui gatewayEui = await GetGatewayEui().ConfigureAwait(false);
                WriteLog("The gateway EUI: " + gatewayEui);


                // Create the LoRa WAN gateway handler:
                ILoraWanGateway loraWanGateway = LoraWanGatewayFactory.Create(
                    transceiver,
                    _gatewaySettings,
                    gatewayEui,
                    positionProvider);

                // Loop forever and send status to the LoRaWAN network periodically:
                while (true)
                {
                    await loraWanGateway.SendStatus().ConfigureAwait(false);

                    await Task.Delay(TimeSpan.FromSeconds(30));
                }
            }
            catch (Exception exception)
            {
                WriteLog("The demo crashed:\r\n" + exception.Message);
                Debugger.Break();
            }
        }

        private async void GpsManagerPositionDataAsync(object sender, PositionDataEventArgs e)
        {
            WriteLog("GPS: " + e.Position);
            await UpdateMapPoint(e.Position).ConfigureAwait(false);
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

                    Debug.WriteLine("WorldMap.ZoomLevel=" + WorldMap.ZoomLevel);
                });
        }

        private void WriteLog(string text)
        {
            Debug.WriteLine(text);
        }
    }
}
