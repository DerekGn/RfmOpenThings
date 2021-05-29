
using System;

namespace RfmOpenThings
{
    internal static class StringExtensions
    {
        public static uint ConvertToUInt(this string value)
        {
            if (value.ToLower().StartsWith("0x"))
            {
                return Convert.ToUInt32(value, 16);
            }
            else if (value.StartsWith('0'))
            {
                return Convert.ToUInt32(value, 8);
            }
            else
            {
                return Convert.ToUInt32(value);
            }
        }
    }
}
