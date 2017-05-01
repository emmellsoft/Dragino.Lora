namespace Dragino.Gps.Messages
{
    public static class PmtkMessages
    {
        public static GpsMessage HotStart => new GpsMessage("PMTK101");

        public static GpsMessage WarmStart => new GpsMessage("PMTK102");

        public static GpsMessage ColdStart => new GpsMessage("PMTK103");

        public static GpsMessage FullColdStart => new GpsMessage("PMTK104");

        public static GpsMessage Standby => new GpsMessage("PMTK161", "0");

        public static GpsMessage EnableEasy => new GpsMessage("PMTK869", "1,1");

        public static GpsMessage DisableEasy => new GpsMessage("PMTK869", "1,0");
    }
}
