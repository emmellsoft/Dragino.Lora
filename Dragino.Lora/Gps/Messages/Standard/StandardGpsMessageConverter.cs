namespace Dragino.Gps.Messages.Standard
{
    internal static class StandardGpsMessageConverter
    {
        public static bool TryConvertToStandardGpsMessage(GpsMessage message, out StandardGpsMessage standardGpsMessage)
        {
            switch (message.MessageId)
            {
                case "GPRMC":
                    standardGpsMessage = new RmcStandardGpsMessage(message);
                    return true;
                case "GPVTG":
                    standardGpsMessage = new VtgStandardGpsMessage(message);
                    return true;
                case "GPGGA":
                    standardGpsMessage = new GgaStandardGpsMessage(message);
                    return true;
                case "GPGSA":
                    standardGpsMessage = new GsaStandardGpsMessage(message);
                    return true;
                case "GPGSV":
                    standardGpsMessage = new GsvStandardGpsMessage(message);
                    return true;
                case "GPGLL":
                    standardGpsMessage = new GllStandardGpsMessage(message);
                    return true;
                case "GPTXT":
                    standardGpsMessage = new TxtStandardGpsMessage(message);
                    return true;
            }

            standardGpsMessage = null;
            return false;
        }
    }
}