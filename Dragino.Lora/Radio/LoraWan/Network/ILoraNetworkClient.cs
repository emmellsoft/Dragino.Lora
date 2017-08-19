using System;
using System.Threading.Tasks;

namespace Dragino.Radio.LoraWan.Network
{
    internal interface ILoraNetworkClient : IDisposable
    {
        Task SendMessage(byte[] buffer);
    }
}