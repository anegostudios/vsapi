using System;

#nullable disable

namespace Vintagestory.API.Util
{
    public class EqualityUtil
    {

        public static bool NumberEquals(object a, object b)
        {
            if ((a is int || a is long) && (b is int || b is long))
            {
                return Convert.ToInt64(a) == Convert.ToInt64(b);
            }

            if ((a is float || a is double) && (b is float || b is double))
            {
                return Math.Abs(Convert.ToDouble(a) - Convert.ToDouble(b)) < 0.00001;
            }

            return false;
        }
    }
}
