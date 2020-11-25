using System;
using Newtonsoft.Json;
using static TimeManager.Utilities.DateExtensions;

namespace TimeManager.Utilities
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Period
    {
        #region constructors

        /// <summary> Start a new unfinished period just now. </summary>
        public Period()
        {
            Start = DateTime.Now;
            //End = DateTime.MinValue; (default value)
        }
        /// <summary> Create a new unfinished period that starts on the specified date. </summary>
        public Period(DateTime start)
        {
            Start = start;
            //End = DateTime.MinValue; (default value)
        }
        public Period(DateTime start, DateTime end)
        {
            Start = start;
            End = end;
        }
        /// <summary> Create a new period that starts specified number of days ago and ends today. </summary>
        public Period(int lastDays)
        {
            Start = DateTime.Today - TimeSpan.FromDays(lastDays);
            End = DateTime.Today;
        }

        #endregion

        [JsonProperty] public DateTime Start { get; set; }
        [JsonProperty] public DateTime End { get; set; }

        private bool IsFinished => End > Start;
        
        public void Finish() => End = DateTime.Now;

        public TimeSpan Duration() => (IsFinished ? End : DateTime.Now ) - Start;
        //public TimeSpan Duration() => End - Start;
        public TimeSpan TimeLeft() => End - DateTime.Now;
        public TimeSpan TimePassed() => DateTime.Now - Start;

        public DateTime StartDate() => Start.Date;
        public DateTime EndDate() => End.Date;

        public override string ToString()
        {
            return Start.Date == End.Date
                ? $"{DateOnly(Start)} {Start.TimeOfDay:%h\\:mm} - {End.TimeOfDay:%h\\:mm}"
                : $"{DateAndTime(Start)} - {(IsFinished ? DateAndTime(End) : "now")}";
        }
        public string ToString(bool datesOnly)
        {
            return Start.Date == End.Date
                ? $"{DateOnly(Start)}"
                : $"{DateOnly(Start)} - {(IsFinished ? DateOnly(End) : "now")}";
        }
    }
}