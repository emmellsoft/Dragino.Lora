namespace Dragino.Radio.LoraWan.Network
{
    internal class NetworkMessage
    {
        public NetworkMessage(string json, NetworkMessageKind messageKind)
        {
            Json = json;
            MessageKind = messageKind;
            Data = new MessageComposer()
        }

        public string Json { get; }

        public NetworkMessageKind MessageKind { get; }

        public byte[] Data { get; }
    }
}