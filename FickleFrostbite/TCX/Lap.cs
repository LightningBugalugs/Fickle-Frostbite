using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace FickleFrostbite.TCX
{
    /// <summary>
    /// <para>.Net representation of an Lap type</para>
    /// </summary>
    [Serializable]
    public class Lap
    {
        [XmlAttribute("StartTime")]
        public string StartTime { get; set; }

        public decimal TotalTimeSeconds { get; set; }
        public decimal DistanceMeters { get; set; }
        public int Calories { get; set; }

        public HeartRateInBeatsPerMinute_t AverageHeartRateBpm { get; set; }
        public HeartRateInBeatsPerMinute_t MaximumHeartRateBpm { get; set; }

        public List<Trackpoint> Track { get; set; }
    }
}
