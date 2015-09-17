using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace FickleFrostbite.TCX
{
    /// <summary>
    /// <para>.Net representation of an Trackpoint type</para>
    /// </summary>
    public class Trackpoint
    {
        public DateTime Time { get; set; }
        public Position Position { get; set; }
        public decimal AltitudeMeters { get; set; }
        public decimal DistanceMeters { get; set; }
        public HeartRateInBeatsPerMinute_t HeartRateBpm { get; set; }
    }
}
