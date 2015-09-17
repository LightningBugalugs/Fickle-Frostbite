using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FickleFrostbite.GoogleMap
{
    /// <summary>
    /// <para>MapPoint object represents a point on a map (Latitude and Logitude)</para>
    /// </summary>
    public class MapPoint
    {
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }

        /// <summary>
        /// <para>Default constructor</para>
        /// </summary>
        public MapPoint() { }

        /// <summary>
        /// <para>Default contructor taking a latitude and logitude values</para>
        /// </summary>
        /// <param name="latitude">Latitude of the MapPoint</param>
        /// <param name="longitude">Longitude of the MapPoint</param>
        public MapPoint(decimal latitude, decimal longitude) : this()
        {
            this.Latitude = latitude;
            this.Longitude = longitude;
        }
    }
}
