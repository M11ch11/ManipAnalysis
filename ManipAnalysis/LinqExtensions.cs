using System;
using System.Collections.Generic;
using System.Linq;

namespace ManipAnalysis_v2
{
    public static class Extensions
    {
        public static double StdDev(this IEnumerable<double> values)
        {
            double ret = 0;
            int count = values.Count();
            if (count > 1)
            {
                //Compute the Average
                double avg = values.Average();

                //Perform the Sum of (value-avg)^2
                double sum = values.Sum(d => (d - avg)*(d - avg));

                //Divide Sum by (Samples - 1)
                ret = Math.Sqrt(sum/(count - 1));
            }
            return ret;
        }

        public static double StdDev(this IEnumerable<long> values)
        {
            double ret = 0;
            int count = values.Count();
            if (count > 1)
            {
                //Compute the Average
                double avg = values.Average();

                //Perform the Sum of (value-avg)^2
                double sum = values.Sum(d => (d - avg)*(d - avg));

                //Divide Sum by (Samples - 1)
                ret = Math.Sqrt(sum/(count - 1));
            }
            return ret;
        }

        public static double[] ArrayAverage(this List<double[]> arrays)
        {
            //Checks wether all arrays are the same size
            var arrayLength = arrays.Select(a => a.Length).Distinct().Single();

            return Enumerable.Range(0, arrays[0].Length)
                       .Select(i => arrays.Select(a => a.Skip(i).First()).Average())
                       .ToArray();
        }
    }
}