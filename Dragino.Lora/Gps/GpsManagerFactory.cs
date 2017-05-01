using System.Threading.Tasks;

namespace Dragino.Gps
{
    /// <summary>
    /// A factory class for getting access to the <see cref="IGpsManager"/>.
    /// </summary>
    public static class GpsManagerFactory
    {
        /// <summary>
        /// Create an instance of <see cref="IGpsManager"/>.
        /// </summary>
        /// <param name="settings">The settings of <see cref="IGpsManager"/>.</param>
        /// <returns></returns>
        public static Task<IGpsManager> Create(GpsManagerSettings settings)
        {
            return GpsManager.Create(settings);
        }
    }
}