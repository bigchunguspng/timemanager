using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using static TimeManager.Utilities.DateExtensions;

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
            return Start.Date == End.Date
                ? $"{DateOnly(Start)} {Start.TimeOfDay:%h\\:mm} - {End.TimeOfDay:%h\\:mm}"
                : $"{DateAndTime(Start)} - {(Start > End ? "now" : DateAndTime(End))}";
        }
    }
}