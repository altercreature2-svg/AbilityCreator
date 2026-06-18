using System;
using System.Collections;
using System.Linq;

public static class EnumerableHelper
{
    public static object[] ToArrayIfEnumerable(object obj)
    {
        if (obj is IEnumerable enumerable && !(obj is string))
        {
            return enumerable.Cast<object>().ToArray();
        }

        return null;
    }
}
