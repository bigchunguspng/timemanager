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
                return $"{time.Days} days {a}";                           //2 days - 386 days ...
            if (time.Days > 0)
                return $"day & {TimeSpanToString(time - TimeSpan.FromDays(1))} {a}";    //day & 0:00 - day & 23:59 hours
            if (time.Hours > 0)
                return time.ToString(@"%h\:mm") + $" hours {a}";  //1:00 h - 23:59
            if (time.Minutes > 9)
                return time.ToString(@"%m") + $" minutes {a}";    //10 minutes - 59 minutes
            return time.ToString(@"%m\:ss") + $" {a}";            //0:00 - 9:59
        }

        public static string DateAndTime(DateTime dateTime) => $"{DateOnly(dateTime)} {dateTime.TimeOfDay:%h\\:mm}";
        public static string DateOnly(DateTime dateTime) => dateTime.Date.ToString("dd MMM yyyy", CultureInfo.CurrentCulture);
    }
}