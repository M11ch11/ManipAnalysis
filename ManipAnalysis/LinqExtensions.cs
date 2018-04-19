using System;
using System.Collections.Generic;
using System.Linq;

namespace ManipAnalysis_v2
{
    public static class Extensions
    {
      
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
    }
}