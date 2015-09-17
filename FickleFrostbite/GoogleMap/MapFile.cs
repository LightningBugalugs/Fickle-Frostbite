using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FickleFrostbite.GoogleMap
{
    /// <summary>
    /// <para>MapFile represents a htm file for a google map</para>
    /// </summary>
    public class MapFile
    {
        /// <summary>
        /// <para>The FileInfo information for the creation of the mapFile</para>
        /// </summary>
        protected FileInfo BaseFileInfo { get; set; }

        /// <summary>
        /// <para>MapFile contructor</para>
        /// </summary>
        /// <param name="fileName">Full filename for the map file</param>
        public MapFile(string fileName)
        {
            this.BaseFileInfo = new FileInfo(fileName);
        }
        
        /// <summary>
        /// <para>Generate a Google Map htm file from a list of mapPoints</para>
        /// </summary>
        /// <param name="mapPoints">List of Map Points to generate a google map for</param>
        /// <remarks>
        /// <para>google maps api: http://code.google.com/apis/maps/documentation/javascript/</para>
        /// </remarks>
        public void GenerateMap(List<MapPoint> mapPoints)
        {
            /* create an array of points for use in calculating centroid for the map */
            var points = new decimal[mapPoints.Count][];
            var k = 0;
            foreach (var mapPoint in mapPoints)
            {
                var newPoint = new decimal[2];
                newPoint[0] = mapPoint.Latitude;
                newPoint[1] = mapPoint.Longitude;
                points[k] = newPoint;
                k++;
            }

            /* generate the starting/middle point for the map to be generated around */
            var startingPoint = FickleFrostbite.Math.CalculateCentroid(points);

            /* generate the text for the google map using string builder */
            var googleMap = new StringBuilder();

            /* generate header for the htm file */
            googleMap.AppendLine(@"<!DOCTYPE html>");
            googleMap.AppendLine(@"<html>");
            googleMap.AppendLine(@"<head>");
            googleMap.AppendLine(@"<meta name=""viewport"" content=""initial-scale=1.0, user-scalable=no"" />");
            googleMap.AppendLine(@"<style type=""text/css"">");
            googleMap.AppendLine(@"html { height: 100% }");
            googleMap.AppendLine(@"body { height: 100%; margin: 0; padding: 0 }");
            googleMap.AppendLine(@"#map_canvas { height: 100% }");
            googleMap.AppendLine(@"</style>");
            googleMap.AppendLine(@"<script type=""text/javascript""");
            googleMap.AppendLine(@"src=""http://maps.googleapis.com/maps/api/js?sensor=true"">");
            googleMap.AppendLine(@"</script>");
            googleMap.AppendLine(@"<script type=""text/javascript"">");
            googleMap.AppendLine(@"function initialize() {");
            googleMap.AppendLine(@" var startPoint = new google.maps.LatLng(" + startingPoint[0] + "," + startingPoint[1] + ");");
            googleMap.AppendLine(@" var myOptions = {");
            googleMap.AppendLine(@"     zoom: 14,");
            googleMap.AppendLine(@"     center: startPoint,");
            googleMap.AppendLine(@"     mapTypeId: google.maps.MapTypeId.ROADMAP");
            googleMap.AppendLine(@"};");
            googleMap.AppendLine(@"var map = new google.maps.Map(document.getElementById(""map_canvas""), myOptions);");
            googleMap.AppendLine(@"var startPin = 'http://noPin/noPin.png';");
            googleMap.AppendLine(@"var startMarker = new google.maps.Marker ({ position: startPoint, map: map, icon: startPin });");

            /* generate the google mapping point coordinates and the route map */
            googleMap.AppendLine(@"var routeMapCoordinates = [ ");
            foreach (var mapPoint in mapPoints)
            {
                googleMap.AppendLine(@"new google.maps.LatLng(" + mapPoint.Latitude + ", " + mapPoint.Longitude + "), ");
            }
            googleMap.Remove(googleMap.Length - 4, 4);
            googleMap.AppendLine(@"];");
            googleMap.AppendLine(@"var routeMap = new google.maps.Polyline({ path: routeMapCoordinates, strokeColor: ""#FF0000"", strokeOpacity: 1.0, strokeWeight: 2 });");
            googleMap.AppendLine(@"routeMap.setMap(map);");

            /* generate footer for the htm file */
            googleMap.AppendLine(@"}");
            googleMap.AppendLine(@"</script>");
            googleMap.AppendLine(@"</head>");
            googleMap.AppendLine(@"<body onload=""initialize()"">");
            googleMap.AppendLine(@"<div id=""map_canvas"" style=""width:100%; height:100%""></div>");
            googleMap.AppendLine(@"</body>");
            googleMap.AppendLine(@"</html>");

            /* write the map out to the file */
            if (this.BaseFileInfo.Exists) { this.BaseFileInfo.Delete(); }
            System.IO.StreamWriter file = new System.IO.StreamWriter(this.BaseFileInfo.FullName);
            file.WriteLine(googleMap);
            file.Close();
        }
    }
}
