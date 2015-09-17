using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace FickleFrostbite.TCX
{
    /// <summary>
    /// <para>.Net representation of an Activity type</para>
    /// </summary>
    [Serializable]
    public class Activity
    {
        public string Id { get; set; }
        
        [XmlAttribute("Sport")]
        public string Sport { get; set; }
        
        [XmlElement("Lap")]
        public List<Lap> Laps { get; set; }

        /// <summary>
        /// <para>Provides the total count (laps * trackpoints) of data items for the activity</para>
        /// </summary>
        public int RowCount
        {
            get
            {
                var rowCount = 0;

                foreach (var lap in this.Laps)
                {
                    foreach (var track in lap.Track)
                    {
                        rowCount++;
                    }
                }

                return rowCount;
            }
        }

        /// <summary>
        /// <para>Convert Activity to a list of GoogleMap.MapPoint</para>
        /// </summary>
        /// <returns>
        /// <para>List of GoogleMap.MapPoint representing the activity.</para>
        /// </returns>
        public List<FickleFrostbite.GoogleMap.MapPoint> ToMapPoints()
        {
            var mapPoints = new List<FickleFrostbite.GoogleMap.MapPoint>();

            foreach (var lap in this.Laps)
            {
                foreach (var track in lap.Track)
                {
                    if (track.Position != null)
                    {
                        var mapPoint = new FickleFrostbite.GoogleMap.MapPoint(track.Position.LatitudeDegrees, track.Position.LongitudeDegrees);
                        mapPoints.Add(mapPoint);
                    }
                }
            }

            return mapPoints;
        }
    }
}
