namespace Dragino.Gps.Messages
{
    public class GpsMessage
    {
        public GpsMessage(string messageId, params string[] dataFields)
        {
            MessageId = messageId;
            DataFields = dataFields;
        }

        public string MessageId { get; }

        public string[] DataFields { get; }

        public override string ToString() => $"{MessageId}({string.Join(",", DataFields)})";
    }
}