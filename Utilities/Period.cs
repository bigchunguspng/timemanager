using System;
using System.Globalization;

namespace TimeManager.Utilities
{
    public class Period
    {
        private bool _finished;

        #region constructors

        public Period()
        {
            Start = DateTime.Now;
        }
        public Period(DateTime when)
        {
            Start = when.Date;
        }
        public Period(DateTime start, DateTime end)
        {
            Start = start;
            End = end;
        }

        #endregion

        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public bool Finished
        {
            get => End > Start;
            set => _finished = value;
        }

        public void Finish()
        {
            End = DateTime.Now;
            Finished = true;
        }

        public TimeSpan Duration() => End - Start;
        public TimeSpan TimeLeft() => End - DateTime.Now;
        public TimeSpan TimePassed() => DateTime.Now - Start;

        public override string ToString()
        {
            return $"{Start.ToString(CultureInfo.CurrentCulture)} - {End.ToString(CultureInfo.CurrentCulture)}";
        }

        public static string TimeSpanToString(TimeSpan time, string a = "")
        {
            /*if (time.Days > 365)
            {
                
            }*/
            if (time.Days > 1)
            {
                return $"{time.Days} days {a}";
            }
            /*if (time.Days == 1)
            {
                //return $"Day and {time.Hours} hours ago";
            }*/
            if (time.Hours > 1)
            {
                return time.ToString(@"%h\:mm\:ss") + $" {a}";
            }
            else
            {
                return time.ToString(@"%m\:ss") + $" {a}";
            }
        }
    }
}