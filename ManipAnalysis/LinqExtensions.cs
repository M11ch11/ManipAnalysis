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
            var doubles = values as IList<double> ?? values.ToList();
            if (doubles.Count > 1)
            {
                //Compute the Average
                var avg = doubles.Average();

                //Perform the Sum of (value-avg)^2
                var sum = doubles.Sum(d => (d - avg)*(d - avg));

                //Divide Sum by (Samples - 1)
                ret = Math.Sqrt(sum/(doubles.Count - 1));
            }
            return ret;
        }

        public static double StdDev(this IEnumerable<long> values)
        {
            double ret = 0;
            var longs = values as IList<long> ?? values.ToList();
            if (longs.Count > 1)
            {
                //Compute the Average
                var avg = longs.Average();

                //Perform the Sum of (value-avg)^2
                var sum = longs.Sum(d => (d - avg)*(d - avg));

                //Divide Sum by (Samples - 1)
                ret = Math.Sqrt(sum/(longs.Count - 1));
            }
            return ret;
        }


        public static double[] ArrayAverage(List<double[]> arrays)
        {
            //Checks wether all arrays are the same size
            var arrayLength = arrays.Select(a => a.Length).Distinct().Single();

            return
                Enumerable.Range(0, arrays[0].Length)
                    .Select(i => arrays.Select(a => a.Skip(i).First()).Average())
                    .ToArray();
        }

        public static long[] ArrayAverage(List<long[]> arrays)
        {
            //Checks wether all arrays are the same size
            var arrayLength = arrays.Select(a => a.Length).Distinct().Single();

            return
                Enumerable.Range(0, arrays[0].Length)
                    .Select(i => arrays.Select(a => a.Skip(i).First()).Average())
                    .Cast<long>()
                    .ToArray();
        }
    }
}