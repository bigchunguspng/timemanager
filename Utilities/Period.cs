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
            //End = DateTime.MinValue;
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

        [JsonIgnore] public bool Finished => End > Start;
        
        public void Finish() => End = DateTime.Now;

        //public TimeSpan Duration() => (Finished ? End : DateTime.Now ) - Start;
        public TimeSpan Duration() => End - Start;
        public TimeSpan TimeLeft() => End - DateTime.Now;
        public TimeSpan TimePassed() => DateTime.Now - Start;

        public DateTime ToDateTime() => Start.Date;

        public override string ToString()
        {
            return Start.Date == End.Date
                ? $"{DateOnly(Start)} {Start.TimeOfDay:%h\\:mm} - {End.TimeOfDay:%h\\:mm}"
                : $"{DateAndTime(Start)} - {(Finished ? DateAndTime(End) : "now")}";
        }
    }
}