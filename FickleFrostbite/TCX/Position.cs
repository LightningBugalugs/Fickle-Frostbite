using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace FickleFrostbite.TCX
{
    /// <summary>
    /// <para>.Net representation of an Position type</para>
    /// </summary>
    public class Position
    {
        public decimal LatitudeDegrees { get; set; }
        public decimal LongitudeDegrees { get; set; }
    }
}
