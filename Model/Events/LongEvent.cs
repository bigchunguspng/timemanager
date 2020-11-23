using System;
using Newtonsoft.Json;
using TimeManager.Model.Tasks;
using TimeManager.Utilities;

namespace TimeManager.Model.Events
{
    [JsonObject(MemberSerialization.OptIn)]
    public class LongEvent : IEvent
    {
        #region constructors

        public LongEvent(string description, Period period)
        {
            Description = description;
            Period = period;
        }
        public LongEvent(Task task)
        {
            Description = task.Description;
            Period = task.Performance;
        }
        public LongEvent(ShortEvent shortEvent)
        {
            Description = shortEvent.Description;
            Period = new Period(shortEvent.Date);
        }

        #endregion

        [JsonProperty] public string Description { get; set; }
        [JsonProperty] public Period Period { get; set; }
        /*
         day (today / yesterday / any)
         days (dayA - dayB / dayA - now)
        */
        public TimeSpan TimeFromStart => Period.TimePassed();
        public TimeSpan TimeFromEnd => - Period.TimeLeft();
        public TimeSpan Duration => Period.Duration();

    }
}