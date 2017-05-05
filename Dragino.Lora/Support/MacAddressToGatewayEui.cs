using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Data.Json;
using Dragino.Radio.LoraWan;

namespace Dragino.Support
{
    /// <summary>
    /// Helper class that gets the MAC address of a Windows 10 IoT Core device and turn it into a GatewayEui.
    /// </summary>
    public static class MacAddressToGatewayEui
    {
        public static async Task<GatewayEui> GetGatewayEui(NetworkCredential piUser)
        {
            string macAddress = await GetMacAddress(piUser).ConfigureAwait(false);

            // (The MAC address is retreived on the form "XX-XX-XX-XX-XX-XX" where X is a hexadecimal character.)

            if (macAddress?.Length != 17)
            {
                throw new Exception($"Bad MAC address found: \"{macAddress}\"");
            }

            // Convert the six byte long MAC address to an eight byte long EUI:
            byte[] eui =
            {
                GetByteFromHexString(macAddress, 0),
                GetByteFromHexString(macAddress, 3),
                GetByteFromHexString(macAddress, 6),
                0xff,
                0xff,
                GetByteFromHexString(macAddress, 9),
                GetByteFromHexString(macAddress, 12),
                GetByteFromHexString(macAddress, 15),
            };

            return new GatewayEui(eui);
        }

        private static byte GetByteFromHexString(string hexString, int index)
        {
            return Convert.ToByte(hexString.Substring(index, 2), 16);
        }

        private static async Task<string> GetMacAddress(NetworkCredential piUser)
        {
            try
            {
                using (var httpClientHandler = new HttpClientHandler { Credentials = piUser })
                {
                    using (var httpClient = new HttpClient(httpClientHandler))
                    {
                        string jsonString = await httpClient.GetStringAsync("http://localhost:8080/api/networking/ipconfig").ConfigureAwait(false);

                        JsonObject resultData = JsonObject.Parse(jsonString);

                        JsonArray adapters = resultData.GetNamedArray("Adapters");

                        for (uint index = 0; index < adapters.Count; index++)
                        {
                            JsonObject adapter = adapters.GetObjectAt(index).GetObject();

                            string type = adapter.GetNamedString("Type");

                            if (string.Equals(type, "ethernet", StringComparison.OrdinalIgnoreCase))
                            {
                                return adapter.GetNamedString("HardwareAddress");
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("GetMacAddress failed: " + e.Message);
                throw;
            }

            throw new Exception("No MAC address found!");
        }
    }
}