using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace L24CM.Utility
{
    public static class DateTimeX
    {
        public static DateTime UnixEra = new DateTime(1970, 1, 1);

        public static DateTime FromUnix(string unixTimestamp)
        {
            return UnixEra.AddSeconds(double.Parse(unixTimestamp));
        }

        public static string ToUnix(this DateTime dt)
        {
            return (dt - UnixEra).TotalSeconds.ToString();
        }
    }
}
