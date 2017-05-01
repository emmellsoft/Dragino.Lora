namespace Dragino.Radio.LoraWan.Network
{
    internal enum SendMessageKind : byte
    {
        PushData = 0x00,
        PushAck = 0x01,
        PullData = 0x02,
        PullResp = 0x03,
        PullAck = 0x04,
        TxAck = 0x05
    }
}