using System;
using System.Threading.Tasks;
using Dragino.Gps.Messages;

namespace Dragino.Gps
{
    public interface IGpsManager : IPositionProvider
    {
        /// <summary>
        /// Event fired when a standard GPS message arrives.
        /// </summary>
        event EventHandler<StandardGpsMessageEventArgs> OnStandardMessage;

        /// <summary>
        /// Event fired when a custom GPS message arrives.
        /// </summary>
        event EventHandler<GpsMessageEventArgs> OnCustomMessage;

        /// <summary>
        /// Event fired when the <see cref="GpsManager.LatestPosition"/> changes.
        /// </summary>
        event EventHandler<PositionDataEventArgs> OnPositionData;

        /// <summary>
        /// Event fired when the <see cref="GpsManager.LatestMovement"/> changes.
        /// </summary>
        event EventHandler<MovementDataEventArgs> OnMovementData;

        /// <summary>
        /// The difference between the system clock and the GPS timestamp.
        /// A positive value means that the system clock > GPS clock.
        /// </summary>
        TimeSpan? UtcDateTimeDifference { get; }

        /// <summary>
        /// Current UTC date time, adjusted to the latest GPS timestamp.
        /// </summary>
        DateTime AdjustedUtcNow { get; }

        /// <summary>
        /// Last known position information.
        /// </summary>
        PositionData LatestPosition { get; }

        /// <summary>
        /// Last known movement information.
        /// </summary>
        MovementData LatestMovement { get; }

        /// <summary>
        /// Send a message to the GPS chip.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        /// <returns></returns>
        Task SendMessage(GpsMessage message);

        /// <summary>
        /// Put the GPS in stand-by mode.
        /// </summary>
        Task Standby();

        /// <summary>
        /// Wake up the GPS from stand-by mode.
        /// </summary>
        Task WakeUp();
    }
}