namespace Dragino.Gps.Messages.Standard
{
    public abstract class StandardGpsMessage
    {
        protected StandardGpsMessage(StandardGpsMessageKind kind)
        {
            Kind = kind;
        }

        /// <summary>
        /// The kind of message.
        /// </summary>
        public StandardGpsMessageKind Kind { get; }
    }
}