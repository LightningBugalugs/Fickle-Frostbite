using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FickleFrostbite
{
    public static class Math
    {
        public static decimal[] CalculateCentroid(decimal[][] points)
        {
            decimal[] centroid = null;

            var pointsCount = points.Length;
            if (points.Length > 0)
            {
                var dimensionality = points[0].Length;

                centroid = new decimal[dimensionality];
                foreach (var point in points)
                {
                    for (int i = 0; i < point.Length; i++)
                    {
                        centroid[i] += point[i];
                    }
                }

                for (int i = 0; i < dimensionality; i++)
                {
                    centroid[i] = centroid[i] / pointsCount;
                }
            }

            return centroid;
        }

        public static double CalculateEuclideanDistance(double[] a, double[] b)
        {
            if (a.Length != b.Length) { throw new Exception("lengths of a and b must be equal to calculate euclidean distance"); }

            var euclideanDistance = 0.0;
            for (int i = 0; i < a.Length; i++)
            {
                euclideanDistance += System.Math.Pow((a[i] - b[i]), 2);
            }
            euclideanDistance = System.Math.Sqrt(euclideanDistance);

            return euclideanDistance;
        }
    }
}
