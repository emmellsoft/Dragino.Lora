namespace Dragino.Radio.LoraWan
{
    public class LoraWanGatewaySettings : TransceiverSettings
    {
        public static class TheThingsNetwork
        {
            public static readonly LoraWanGatewaySettings Europe868 = new LoraWanGatewaySettings(868100000, SpreadingFactor.SF7, new[] { "router.eu.thethings.network" }, 1700);
            public static readonly LoraWanGatewaySettings Europe433 = new LoraWanGatewaySettings(433000000, SpreadingFactor.SF7, new[] { "40.114.249.243", "router.eu.thethings.network" }, 1700);
            public static readonly LoraWanGatewaySettings UnitedStates = new LoraWanGatewaySettings(915000000, SpreadingFactor.SF7, new[] { "router.us.thethings.network" }, 1700);
        }

        public LoraWanGatewaySettings(
            uint frequency,
            SpreadingFactor spreadingFactor,
            string[] hosts,
            int port)
            : base(
                RadioModemKind.Lora,
                frequency,
                BandWidth.BandWidth_125_00_kHz,
                spreadingFactor,
                CodingRate.FourOfFive,
                8,
                true,
                false,
                LoraSyncWord.Public)
        {
            Hosts = hosts;
            Port = port;
        }

        public string[] Hosts { get; }

        public int Port { get; }
    }
}