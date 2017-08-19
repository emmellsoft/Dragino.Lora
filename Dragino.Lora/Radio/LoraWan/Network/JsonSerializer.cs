using Dragino.Radio.LoraWan.Network.Messages;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Dragino.Radio.LoraWan.Network
{
    internal static class JsonSerializer
    {
        private class CustomDateTimeConverter : IsoDateTimeConverter
        {
            public CustomDateTimeConverter(string format)
            {
                DateTimeFormat = format;
            }
        }

        private class StatMessageWrapper
        {
            public StatMessageWrapper(StatMessage message)
            {
                Message = message;
            }

            [JsonProperty("stat")]
            public StatMessage Message { get; }
        }

        private class RxpkMessageWrapper
        {
            public RxpkMessageWrapper(RxpkMessage message)
            {
                Messages = new[] { message };
            }

            [JsonProperty("rxpk")]
            public RxpkMessage[] Messages { get; }
        }

        private static readonly CustomDateTimeConverter _statMessageDateTimeConverter = new CustomDateTimeConverter("yyyy'-'MM'-'dd' 'HH':'mm':'ss' GMT'");
        private static readonly CustomDateTimeConverter _rxpkMessageDateTimeConverter = new CustomDateTimeConverter("o");

        public static string ToJson(StatMessage message)
        {
            return JsonConvert.SerializeObject(new StatMessageWrapper(message), Formatting.None, _statMessageDateTimeConverter);
        }

        public static string ToJson(RxpkMessage message)
        {
            return JsonConvert.SerializeObject(new RxpkMessageWrapper(message), Formatting.None, _rxpkMessageDateTimeConverter);
        }
    }
}