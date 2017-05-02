namespace Dragino.Gps
{
    /// <summary>
    /// The various standard message kinds.
    /// </summary>
    public enum StandardGpsMessageKind
    {
        /// <summary>
        /// Recommended minimum position data (including position, velocity and time).
        /// </summary>
        Rmc,

        /// <summary>
        /// Track made good and ground speed. 
        /// </summary>
        Vtg,

        /// <summary>
        /// Global positioning system fix data, is the essential fix data which provides 3D location and accuracy data. 
        /// </summary>
        Gga,

        /// <summary>
        /// GNSS DOP and Active Satellites, provides details on the fix, including the numbers of the satellites being used and the DOP. At most the first 12 satellite IDs are output.
        /// </summary>
        Gsa,

        /// <summary>
        /// GNSS Satellites in View. One GSV sentence can only provide data for at most 4 satellites, so several sentences might be required for the full information. Since GSV includes satellites that are not used as part of the solution, GSV sentence contains more satellites than GGA does.
        /// </summary>
        Gsv,

        /// <summary>
        /// Geographic Latitude and Longitude, contains position information, time of position fix and status.
        /// </summary>
        Gll,

        /// <summary>
        /// This message is used to ouput information.
        /// </summary>
        Txt
    }
}