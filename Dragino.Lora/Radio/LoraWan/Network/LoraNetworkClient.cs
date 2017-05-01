using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace Dragino.Radio.LoraWan.Network
{
    internal class LoraNetworkClient : IDisposable
    {
        private readonly EndpointPair _endpointPair;
        private readonly DatagramSocket _udp;

        public LoraNetworkClient(HostName hostName, int port)
        {
            _endpointPair = new EndpointPair(
                null,
                string.Empty,
                hostName,
                port.ToString());

            _udp = new DatagramSocket();
            _udp.Control.DontFragment = true;
            _udp.MessageReceived += UdpMessageReceived;
        }

        public void Dispose()
        {
            _udp.MessageReceived -= UdpMessageReceived;
            _udp.Dispose();
        }
        
        public async Task SendMessage(IBuffer buffer)
        {
            try
            {
                using (IOutputStream outputStream = await _udp.GetOutputStreamAsync(_endpointPair))
                {
                    byte[] bytes = buffer.ToArray();
                    Debug.WriteLine($"UDP sending (to \"{_endpointPair.RemoteHostName}\"): {string.Join(", ", bytes.Select(x => x.ToString("X2")))} ({bytes.Length} byte(s)).");

                    await outputStream.WriteAsync(buffer);
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine("UDP send exception: " + exception);
            }
        }

        private void UdpMessageReceived(DatagramSocket sender, DatagramSocketMessageReceivedEventArgs args)
        {
            try
            {
                using (DataReader dataReader = args.GetDataReader())
                {
                    IBuffer buffer = dataReader.ReadBuffer(dataReader.UnconsumedBufferLength);
                    byte[] bytes = buffer.ToArray();

                    Debug.WriteLine($"UDP receiving (from \"{args.RemoteAddress}\"): {string.Join(", ", bytes.Select(x => x.ToString("X2")))} ({bytes.Length} byte(s)).");
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine("UDP receive exception: " + exception);
            }
        }
    }
}