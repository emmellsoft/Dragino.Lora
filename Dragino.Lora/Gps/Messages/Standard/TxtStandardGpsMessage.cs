namespace Dragino.Gps.Messages.Standard
{
    /// <summary>
    /// This message is used to ouput information.
    /// </summary>
    public class TxtStandardGpsMessage : StandardGpsMessage
    {
        public TxtStandardGpsMessage(GpsMessage message)
            : base(StandardGpsMessageKind.Txt)
        {
            // Example of data fields:
            // "01", "01", "02", "ANTSTATUS=OK"

            NumberOfMessages = GpsValueParser.ParseNullableInt(message.DataFields[0]) ?? 1;
            SequenceNumber = GpsValueParser.ParseNullableInt(message.DataFields[1]) ?? 1;
            Severity = GpsValueParser.ParseTextMessageSeverity(message.DataFields[2]);
            TextMessage = message.DataFields[3];
        }

        /// <summary>
        /// Total number of messages in this transmission (1-99).
        /// </summary>
        public int NumberOfMessages { get; }

        /// <summary>
        /// Message number in this transmission (1-99).
        /// </summary>
        public int SequenceNumber { get; }

        /// <summary>
        /// Severity of the message.
        /// </summary>
        public TextMessageSeverity Severity { get; }

        /// <summary>
        /// The output information.
        /// </summary>
        public string TextMessage { get; }

        public override string ToString()
        {
            return $"{Kind}: Mess#=\"{NumberOfMessages}\", Seq#=\"{SequenceNumber}\", Sev=\"{Severity}\", Mess=\"{TextMessage}\"";
        }
    }
}