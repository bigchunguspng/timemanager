using System;

namespace TimeManager
{
    public class Period
    {
        private bool _finished;

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

        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public bool Finished
        {
            get => End > Start;
            set => _finished = value;
        }
    }
}