using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManipAnalysis
{
    class ArrayComparer : IEqualityComparer<int[]>
    {
        public bool Equals(int[] x, int[] y)
        {
            //Check whether the compared objects reference the same data.
            if (Object.ReferenceEquals(x, y)) return true;

            //Check whether any of the compared objects is null.
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            if (x.Length != y.Length)
                return false;

            //Check whether the arrays' values are equal.
            for (int i = 0; i < x.Length; i++)
            {
                if (x[i] != y[i])
                    return false;
            }

            // If got this far, arrays are equal
            return true;
        }

        // If Equals() returns true for a pair of objects 
        // then GetHashCode() must return the same value for these objects.

        public int GetHashCode(int[] intArray)
        {
            //Check whether the object is null
            if (Object.ReferenceEquals(intArray, null)) return 0;

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
