namespace Dragino.Gps.Messages
{
    /// <summary>
    /// A generic GPS message.
    /// </summary>
    public class GpsMessage
    {
        public GpsMessage(string messageId, params string[] dataFields)
        {
            MessageId = messageId;
            DataFields = dataFields;
        }

        /// <summary>
        /// The message ID.
        /// </summary>
        public string MessageId { get; }

        /// <summary>
        /// The data fields.
        /// </summary>
        public string[] DataFields { get; }

        public override string ToString() => $"{MessageId}({string.Join(",", DataFields)})";
    }
}