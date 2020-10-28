using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace TimeManager.Utilities
{
    public static class DateExtensions
    {
        public static TimeSpan SumOf(IEnumerable<Period> periods) =>
            periods?.Aggregate(new TimeSpan(), (current, period) => current + period.Duration()) ?? TimeSpan.Zero;

        public static string TimeSpanToString(TimeSpan time, string a = "")
        {
            if (time.Days > 1)
                return $"{time.Days} days {a}";
            if (time.Hours > 0)
                return time.ToString(@"%h\:mm") + $" hours {a}";
            if (time.Minutes > 9)
                return time.ToString(@"%m") + $" minutes {a}";
            return time.ToString(@"%m\:ss") + $" {a}";
        }

        public static string DateAndTime(DateTime dateTime) => $"{DateOnly(dateTime)} {dateTime.TimeOfDay:%h\\:mm}";
        public static string DateOnly(DateTime dateTime) => dateTime.Date.ToString("dd MMM yyyy", CultureInfo.CurrentCulture);
    }
}