using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Dragino.Radio.LoraWan.Network;

namespace Dragino.Lora.Fake.Gateway
{
    internal class LoraNetworkClient : ILoraNetworkClient
    {
        private readonly IPEndPoint _ipEndPoint;

        public LoraNetworkClient(string host, int port)
        {
            var ipAddress = Dns.GetHostEntry(host).AddressList.First();
            _ipEndPoint = new IPEndPoint(ipAddress, port);
        }

        public void Dispose()
        {
        }

        public async Task SendMessage(byte[] buffer)
        {
            using (UdpClient udpClient = new UdpClient())
            {
                udpClient.Connect(_ipEndPoint);
                await udpClient.SendAsync(buffer, buffer.Length);
            }
        }
    }
}