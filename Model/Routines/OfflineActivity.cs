using System;
using System.Collections.Generic;
using TimeManager.Model.Events;
using TimeManager.Utilities;

namespace TimeManager.Model.Routines
{
    public class OfflineActivity : Routine
    {
        public OfflineActivity(string description) : base(description) { }
        public OfflineActivity(Event @event)
        {
            Description = @event.Description;
            Times = new List<Period> {@event.Period};
        }

        public void AddDate(DateTime date) => Times.Add(new Period(date.Date));

        //public void RemoveDate()
    }
}