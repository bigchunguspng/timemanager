using System;
using Newtonsoft.Json;
using static TimeManager.Utilities.DateExtensions;

namespace TimeManager.Utilities
{
    public class Period
    {
        #region constructors

        public Period()
        {
            Start = DateTime.Now;
            //End = DateTime.MinValue; (default value)
        }
        public Period(DateTime day)
        {
            Start = day.Date;
            End = day.Date;
        }
        public Period(DateTime start, DateTime end)
        {
            Start = start;
            End = end;
        }

        public Period(int lastDays)
        {
            Start = DateTime.Today - TimeSpan.FromDays(lastDays);
            End = DateTime.Today;
        }

        #endregion

        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        [JsonIgnore] private bool IsFinished => End > Start;
        
        public void Finish() => End = DateTime.Now;

        //public TimeSpan Duration() => (IsFinished ? End : DateTime.Now ) - Start;
        public TimeSpan Duration() => End - Start;
        public TimeSpan TimeLeft() => End - DateTime.Now;
        public TimeSpan TimePassed() => DateTime.Now - Start;

        public DateTime StartDate() => Start.Date;
        //public DateTime EndDate() => End.Date;

        public override string ToString()
        {
            return Start.Date == End.Date
                ? $"{DateOnly(Start)} {Start.TimeOfDay:%h\\:mm} - {End.TimeOfDay:%h\\:mm}"
                : $"{DateAndTime(Start)} - {(IsFinished ? DateAndTime(End) : "now")}";
        }
    }
}