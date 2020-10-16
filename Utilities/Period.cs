﻿using System;
using System.Globalization;

namespace TimeManager.Utilities
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

        public override string ToString()
        {
            return $"{Start.ToString(CultureInfo.CurrentCulture)} - {End.ToString(CultureInfo.CurrentCulture)}";
        }
    }
}