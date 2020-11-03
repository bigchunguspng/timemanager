using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using TimeManager.Model.Events;
using TimeManager.Model.Tasks;
using TimeManager.Utilities;

namespace TimeManager.Model.Routines
{
    public class OnlineActivity : Routine
    {
        public OnlineActivity(string description) : base(description) { }

        public OnlineActivity(Task task)
        {
            Description = task.Description;
            Times = new List<Period> {task.Performance};
        }
        public OnlineActivity(Event @event)
        {
            Description = @event.Description;
            Times = new List<Period> {@event.Period};
        }

        [JsonIgnore] public bool Performed => Times[Times.Count - 1].Finished;

        private void Start() => Times.Add(new Period());
        private void Finish() => Times[Times.Count - 1].Finish();
        private void ClearLastTime() => Times.Remove(Times[Times.Count - 1]);

        private TimeSpan LastTimeDuration() => Times[Times.Count - 1].Duration();

        private TimeSpan AverageDuration(Period period)
        {
            CalculateFirstAndLastTimes(period);

            TimeSpan total = TimeSpan.Zero;
            for (int i = First; i < Last; i++)
                total += Times[i].Duration();

            return new TimeSpan(total.Ticks / (Last - First));
        }
    }
}