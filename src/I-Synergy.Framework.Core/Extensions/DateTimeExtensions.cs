using System;

namespace ISynergy.Extensions
{
    public static class DateTimeExtensions
    {
        public static string ToJsonString(this DateTime self)
        {
            string result = self.ToUniversalTime().ToString(Constants.DateTimeOffsetFormat);
            return result;
        }

        public static DateTime ToStartOfDay(this DateTime self)
        {
            return new DateTime(self.Year, self.Month, self.Day, 0, 0, 0, 0);
        }

        public static DateTime ToEndOfDay(this DateTime self)
        {
            return new DateTime(self.Year, self.Month, self.Day, 23, 59, 59, 999);
        }

        public static DateTime ToStartOfYear(this DateTime self, int year)
        {
            return new DateTime(year, 1, 1, 0, 0, 0, 0);
        }

        public static DateTime ToEndOfYear(this DateTime self, int year)
        {
            return new DateTime(year, 12, 31, 23, 59, 59, 999);
        }
    }
}