using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace TimeManager.Utilities
{
    public static class DateExtensions
    {
        public static TimeSpan SumOf(IEnumerable<Period> periods) =>
            periods?.Aggregate(new TimeSpan(), (current, period) => current + period.Duration) ?? TimeSpan.Zero;

        /*public static string Duration(Period period)
        {
            int days = period.Duration().Days;
            if (period.End.Day == period.Start.Day)
                return $"{period.End.Month - period.Start.Month} month {TimeSpanToString(period.Duration() - TimeSpan.FromDays(30.4375 * days))}";
            if (days > 1)
                return $"{days} days";
            return "< 1 day";
        }*/
        
        public static string TimeSpanToString(TimeSpan time, string a = "")
        {
            /*if ((DateTime.Today - time).Day == DateTime.Today.Day)
                return "month" + a;
            if (time.Days > 365.25)
                return $"{Math.Round(time.Days / 365.25)}y {TimeSpanToString(TimeSpan.FromDays(time.Days % 365.25))}";
            if (time.Days > 30)
                return $"{Math.Round(time.Days / 30.4375)}m {Math.Round(time.Days % 30.4375)} days {a}";*/
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
        
        public static string DaysAgo(this DateTime dateTime)
        {
            int result = (DateTime.Today - dateTime.Date).Days;
            switch (result)
            {
                case -1:
                    return "Tomorrow";
                case 0:
                    return "Today";
                case 1:
                    return "Yesterday";
                default:
                    return result > 0 ? $"{result} days ago" : $"In {-result} days";
            }
        }

        public static string DateAndTime(this DateTime dateTime) => $"{dateTime.DateOnly()} {dateTime.TimeOfDay:%h\\:mm}";
        public static string DateOnly(this DateTime dateTime) => dateTime.Date.ToString("dd MMM yyyy", CultureInfo.CurrentCulture);
    }
}