namespace Dragino.Gps.Messages.Standard
{
    /// <summary>
    /// The abstract base class for standard GPS message.
    /// Use the <see cref="Kind"/> property to determine what kind of message it is, and cast it
    /// to the corresponding type.
    /// </summary>
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