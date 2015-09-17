using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FickleFrostbite.SQL
{
    public partial class GarminToSqlEntities
    {
        /// <summary>
        /// <para>GarminToSql Entities constructor with connection string parameter.</para>
        /// </summary>
        /// <param name="connectionString">The connection string to connect to the data with</param>
        /// <remarks>Added as EF6 doesn't have the connection string constructor added by default.</remarks>
        public GarminToSqlEntities(string connectionString) : base(connectionString) { }
    }
}
