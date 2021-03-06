//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FickleFrostbite.SQL
{
    using System;
    using System.Collections.Generic;
    
    public partial class Lap
    {
        public Lap()
        {
            this.Trackpoints = new HashSet<Trackpoint>();
        }
    
        public int LapId { get; set; }
        public int ActivityId { get; set; }
        public string StartTime { get; set; }
        public decimal TotalTimeSeconds { get; set; }
        public decimal DistanceMeters { get; set; }
        public int Calories { get; set; }
        public Nullable<int> AverageHeartRateBpm { get; set; }
        public Nullable<int> MaximumHeartRateBpm { get; set; }
    
        public virtual Activity Activity { get; set; }
        public virtual ICollection<Trackpoint> Trackpoints { get; set; }
    }
}
