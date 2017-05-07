using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Dragino.Gps;
using Dragino.Radio;
using Dragino.Radio.LoraWan;
using Dragino.Support;

namespace Dragino.Lora.Demo.Gateway
{
    // *********************************************************************************************
    // For the GPS manager to work (if you have a GPS chip on your Dragino board),
    // the serial communication must be allowed in the application.
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
        // *********************************************************************************************
        // #1/4. YOUR EDITING IS REQUIRED HERE!
        // Choose correct gateway settings:
        private static readonly LoraWanGatewaySettings _gatewaySettings = LoraWanGatewaySettings.TheThingsNetwork.Europe868;
        // *********************************************************************************************

        private readonly ILoraWanGateway _loraWanGateway;
        private readonly TimeSpan _sendStatusInterval = TimeSpan.FromSeconds(30);
        private readonly Timer _sendStatusTimer;

        public MainPage()
        {
            InitializeComponent();

            _loraWanGateway = Task.Run(CreateGateway).ConfigureAwait(false).GetAwaiter().GetResult();

            if (_loraWanGateway != null)
            {
                WriteLog("The LoRaWAN Gateway is created successfully.");
                _sendStatusTimer = new Timer(SendStatusTimerTick, null, TimeSpan.Zero, _sendStatusInterval);
            }
        }

        private static TransceiverPinSettings GetTransceiverPinSettings()
        {
            // *********************************************************************************************
            // #2/4. YOUR EDITING IS REQUIRED HERE!
            // 
            // Depending on the kind of Dragino expansion board you have, uncomment the right line below!
            // *********************************************************************************************


            // EITHER: I have a Dragino LoRa/GPS HAT attached on my Raspberry Pi:
            return TransceiverPinSettings.DraginoLoraGpsHat;


            // OR: I have a Dragino LoRa (GPS) Arduino Shield connected via wires to the GPIO on my Raspberry Pi:
            //return new TransceiverPinSettings(
            //    25,  // ChipSelect
            //    17,  // Reset
            //    4,   // Dio0
            //    23,  // Dio1 (Optional -- you may use null)
            //    24); // Dio2 (Optional -- you may use null)
        }

        private static bool UseGpsManager()
        {
            // *********************************************************************************************
            // #3/4. YOUR EDITING IS REQUIRED HERE!
            // 
            // Depending on the kind of Dragino expansion board you have, uncomment the right line below!
            // *********************************************************************************************


            // EITHER: I am using an Dragino extension board having a GPS chip:
            return true;


            // OR: There is no GPS chip on my Dragino extension board:
            //return false;
        }

        /// <summary>
        /// Get the Gateway EUI.
        /// </summary>
        /// <returns></returns>
        private static Task<GatewayEui> GetGatewayEui()
        {
            // *********************************************************************************************
            // #4/4. YOUR EDITING IS REQUIRED HERE!
            // 
            // Either use a hardcoded EUI or the MAC address of the device (e.g. Raspberry Pi)!
            // *********************************************************************************************
            // EITHER: Use the MAC address:
            var piUser = new System.Net.NetworkCredential("Administrator", "p@ssw0rd"); // <-- Edit the administrator password for your Raspberry Pi here!
            return MacAddressToGatewayEui.GetGatewayEui(piUser);


            //// OR: Use a hard coded EUI:
            //return Task.FromResult(new GatewayEui("0123456789ABCDEF"));
        }

        private async Task<ILoraWanGateway> CreateGateway()
        {
            try
            {
                // Create the transceiver:
                TransceiverPinSettings pinSettings = GetTransceiverPinSettings();
                ITransceiver transceiver = await TransceiverFactory.Create(_gatewaySettings, pinSettings).ConfigureAwait(false);
                transceiver.OnMessageReceived += TransceiverOnMessageReceived;


                // Create the GPS manager (if existing):
                IPositionProvider positionProvider;
                if (UseGpsManager())
                {
                    // Create the GPS manager:
                    IGpsManager gpsManager = await GpsManagerFactory.Create(GpsManagerSettings.Default).ConfigureAwait(false);

                    // Hook up the event fired when a new position is recorded:
                    gpsManager.OnPositionData += GpsManagerPositionDataAsync;

                    // Start the GPS:
                    await gpsManager.WakeUp().ConfigureAwait(false);

                    // Make the Gateway use the GpsManager as position provider
                    // (sending the actual coordinates in its status messages):
                    positionProvider = gpsManager;
                }
                else
                {
                    // Make the Gateway use "no position" as its coordinate:
                    positionProvider = FixedPositionProvider.NoPositionProvider;

                    // ...or give it a fixed coordinate:
                    //positionProvider = new FixedPositionProvider(new SimplePosition(55.597382, 12.95889, 18.4));
                }


                // Get the gateway EUI:
                GatewayEui gatewayEui = await GetGatewayEui().ConfigureAwait(false);
                WriteLog("The gateway EUI: " + gatewayEui);


                // Create the LoRa WAN gateway handler:
                return LoraWanGatewayFactory.Create(
                    transceiver,
                    _gatewaySettings,
                    gatewayEui,
                    positionProvider);
            }
            catch (Exception exception)
            {
                WriteLog("Failed creating the LoRaWAN gateway:\r\n" + exception.Message);
                Debugger.Break();
                return null;
            }
        }

        private async void SendStatusTimerTick(object state)
        {
            // Send the current status of the gateway to the network backend.
            await _loraWanGateway.SendStatus().ConfigureAwait(false);
        }

        private void TransceiverOnMessageReceived(object sender, ReceivedMessageEventArgs e)
        {
            // Here you can handle a received message as you like:
            WriteLog("Message Received: " + e.Message);
        }

        private void GpsManagerPositionDataAsync(object sender, PositionDataEventArgs e)
        {
            // Here you can handle a newly recorded geographical position as you please:
            WriteLog("GPS: " + e.Position);
        }

        private void WriteLog(string text)
        {
            // Simply writing to the Output window:
            Debug.WriteLine(text);
        }
    }
}
