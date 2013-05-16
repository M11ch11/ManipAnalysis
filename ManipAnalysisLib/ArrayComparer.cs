using System.Collections.Generic;
using System.Linq;

namespace ManipAnalysisLib
{
    internal class ArrayComparer : IEqualityComparer<int[]>
    {
        public bool Equals(int[] x, int[] y)
        {
            //Check whether the compared objects reference the same data.
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            //Check whether any of the compared objects is null.
            if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
            {
                return false;
            }

            if (x.Length != y.Length)
            {
                return false;
            }

            //Check whether the arrays' values are equal.
            return !x.Where((t, i) => t != y[i]).Any();

            // If got this far, arrays are equal
        }

        // If Equals() returns true for a pair of objects 
        // then GetHashCode() must return the same value for these objects.

        public int GetHashCode(int[] intArray)
        {
            //Check whether the object is null
            if (ReferenceEquals(intArray, null))
            {
                return 0;
            }

            //Calculate the hash code for the array
            int hashCode = 0;
            bool isFirst = true;
            foreach (int i in intArray)
            {
                if (isFirst)
                {
                    hashCode = i;
                    isFirst = false;
                }
                else
                {
                    hashCode = hashCode ^ i;
                }
            }
            return hashCode;
        }
    }
}