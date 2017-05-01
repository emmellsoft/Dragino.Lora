using Windows.Data.Json;
using Dragino.Radio.LoraWan.Network.Messages;

namespace Dragino.Radio.LoraWan.Network
{
    internal static class JsonSerializer
    {
        public static JsonObject ToJson(StatMessage message)
        {
            return new JsonObject
            {
                { "stat", new JsonObject
                    {
                        { "time", JsonValue.CreateStringValue(message.Time.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss' GMT'")) },
                        { "lati", JsonValue.CreateNumberValue(message.Latitude) },
                        { "long", JsonValue.CreateNumberValue(message.Longitude) },
                        { "alti", JsonValue.CreateNumberValue(message.Altitude) },
                        { "rxnb", JsonValue.CreateNumberValue(message.Rxnb) },
                        { "rxok", JsonValue.CreateNumberValue(message.Rxok) },
                        { "rxfw", JsonValue.CreateNumberValue(message.Rxfw) },
                        { "ackr", JsonValue.CreateNumberValue(message.Ackr) },
                        { "dwnb", JsonValue.CreateNumberValue(message.Dwnb) },
                        { "txnb", JsonValue.CreateNumberValue(message.Txnb) }
                    }
                }
            };
        }

        public static JsonObject ToJson(RxpkMessage message)
        {
            return new JsonObject
            {
                { "rxpk", new JsonArray
                    {
                        new JsonObject
                        {
                            { "time", JsonValue.CreateStringValue(message.Time.ToString("o")) },
                            { "tmst", JsonValue.CreateNumberValue(message.Tmst) },
                            { "freq", JsonValue.CreateNumberValue(message.Freq) },
                            { "chan", JsonValue.CreateNumberValue(message.Chan) },
                            { "rfch", JsonValue.CreateNumberValue(message.Rfch) },
                            { "stat", JsonValue.CreateNumberValue(message.Stat) },
                            { "modu", JsonValue.CreateStringValue(message.Modu) },
                            { "datr", JsonValue.CreateStringValue(message.Datr) },
                            { "codr", JsonValue.CreateStringValue(message.Codr) },
                            { "rssi", JsonValue.CreateNumberValue(message.Rssi) },
                            { "lsnr", JsonValue.CreateNumberValue(message.Lsnr) },
                            { "size", JsonValue.CreateNumberValue(message.Size) },
                            { "data", JsonValue.CreateStringValue(message.Data) }
                        }
                    }
                }
            };
        }
    }
}