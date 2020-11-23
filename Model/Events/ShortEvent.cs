using System;
using Newtonsoft.Json;
using TimeManager.Model.Tasks;

namespace TimeManager.Model.Events
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ShortEvent : IEvent
    {
        private DateTime _date;

        #region constructors

        public ShortEvent(string description, DateTime date)
        {
            Description = description;
            Date = date;
        }

        public ShortEvent(Task task)
        {
            Description = task.Description;
            Date = task.Performance.EndDate();
        }

        #endregion

        [JsonProperty] public string Description { get; set; }
        [JsonProperty] public DateTime Date
        {
            get => _date;
            set => _date = value.Date;
        }
        
        /*
         day (today / yesterday / any)
        */

        public TimeSpan TimeFromStart => TimeFromEnd;
        public TimeSpan TimeFromEnd => DateTime.Today - Date;
        public TimeSpan Duration => TimeSpan.FromDays(1);


    }
}