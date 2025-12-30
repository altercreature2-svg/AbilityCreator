using System;
using System.Collections;
using System.Linq;

public static class EnumerableHelper
{
    /// <summary>
    /// Converts an object into an array if it's IEnumerable.
    /// Returns null if the object is not enumerable.
    /// </summary>
    public static object[] ToArrayIfEnumerable(object obj)
    {
        if (obj is IEnumerable enumerable && !(obj is string)) // exclude string since it's IEnumerable<char>
        {
            return enumerable.Cast<object>().ToArray();
        }

        return null; // not an IEnumerable
    }
}
