using System;
using System.Threading.Tasks;

namespace Dragino.Radio.LoraWan
{
    /// <summary>
    /// An interface for a LoRaWAN Gateway connected to a LoRaWAN server.
    /// </summary>
    public interface ILoraWanGateway : IDisposable
    {
        /// <summary>
        /// Send the current status of the gateway to the server.
        /// </summary>
        Task SendStatus();
    }
}