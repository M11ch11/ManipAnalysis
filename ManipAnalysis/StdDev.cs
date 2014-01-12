using System;
using System.Collections.Generic;
using System.Linq;

namespace ManipAnalysis
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

                //Put it all together
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
    }
}