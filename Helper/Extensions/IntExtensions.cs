using System;

namespace Helper.Extensions
{
    public static class IntExtensions
    {
        public static TimeSpan ToMinutes(this int i)
        {
            return TimeSpan.FromMinutes(i);
        }
        public static TimeSpan ToHours(this int i)
        {
            return TimeSpan.FromHours(i);
        }
        public static TimeSpan ToSeconds(this int i)
        {
            return TimeSpan.FromSeconds(i);
        }
    }
}
