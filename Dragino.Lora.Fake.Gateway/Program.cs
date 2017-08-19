using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dragino.Gps;
using Dragino.Radio;
using Dragino.Radio.LoraWan;
using Dragino.Radio.LoraWan.Network;

namespace Dragino.Lora.Fake.Gateway
{
    internal static class Program
    {
        private static bool _running;

        private static void Main(string[] args)
        {
            FakeTransceiver fakeTransceiver = new FakeTransceiver();

            //GatewayEui gatewayEui = new GatewayEui("b827ebffff39cc04");
            GatewayEui gatewayEui = new GatewayEui("19c683a9649b6129");
            LoraWanGatewaySettings settings = LoraWanGatewaySettings.TheThingsNetwork.Europe868;

            using (LoraWanGateway loraWanGateway = new LoraWanGateway(
                fakeTransceiver,
                settings,
                gatewayEui,
                settings.Hosts.Select(x => new LoraNetworkClient(x, settings.Port))
                    .OfType<ILoraNetworkClient>()
                    .ToArray(),
                FixedPositionProvider.NoPositionProvider))
            {
                Console.WriteLine("Fake server up & running.");

                _running = true;

                Task.Run(async () => await SendStatusLoop(loraWanGateway));

                while (true)
                {
                    Console.WriteLine("Enter fake message to be 'transmitted' (\"exit\" to quit):");
                    Console.Write("> ");
                    string message = Console.ReadLine();
                    if (message == "exit")
                    {
                        break;
                    }

                    byte[] buffer = Encoding.UTF8.GetBytes(message);

                    buffer = Convert.FromBase64String("QD8WASaAEgABo7rauLBZFGdbEVVABvrnrUo=");

                    var receivedMessage = new ReceivedMessage(buffer, -104, -104, 56, true, false, DateTime.UtcNow);

                    fakeTransceiver.FakeReceive(receivedMessage);
                }

                _running = false;
            }
        }

        private static async Task SendStatusLoop(LoraWanGateway loraWanGateway)
        {
            try
            {
                while (_running)
                {
                    Console.WriteLine("Sending status.");
                    await loraWanGateway.SendStatus();

                    DateTime nextSendDateTime = DateTime.UtcNow + TimeSpan.FromSeconds(10);
                    while (_running && (DateTime.UtcNow < nextSendDateTime))
                    {
                        await Task.Delay(100);
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine($"{nameof(SendStatusLoop)} exception: {exception}");
            }
        }
    }
}
