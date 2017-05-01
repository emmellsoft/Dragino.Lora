using Dragino.Gps;

namespace Dragino.Radio.LoraWan
{
    /// <summary>
    /// A factory class for getting access to the <see cref="ILoraWanGateway"/>.
    /// </summary>
    public static class LoraWanGatewayFactory
    {
        /// <summary>
        /// Create an instance of <see cref="ILoraWanGateway"/>.
        /// </summary>
        /// <param name="transceiver">The transceiver.</param>
        /// <param name="gatewaySettings">The gateway settings of <see cref="ILoraWanGateway"/>.</param>
        /// <param name="gatewayEui">The unique Gateway EUI.</param>
        /// <param name="positionProvider">Optional. Interface to a <see cref="IPositionProvider"/>.</param>
        /// <returns></returns>
        public static ILoraWanGateway Create(
            ITransceiver transceiver,
            LoraWanGatewaySettings gatewaySettings,
            GatewayEui gatewayEui,
            IPositionProvider positionProvider = null)
        {
            return new LoraWanGateway(
                transceiver,
                gatewaySettings,
                gatewayEui,
                positionProvider ?? FixedPositionProvider.NoPositionProvider);
        }
    }
}