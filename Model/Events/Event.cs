using Newtonsoft.Json;
using TimeManager.Model.Tasks;
using TimeManager.Utilities;

namespace TimeManager.Model.Events
{
    public class Event
    {
        public Event(string description, Period period)
        {
            Description = description;
            Period = period;
        }

        public Event(Task task)
        {
            Description = task.Description;
            Period = task.Performance;
        }

        [JsonProperty] public string Description { get; set; }
        [JsonProperty] public Period Period { get; }
        /*
         day (today / yesterday / any)
         days (dayA - dayB / dayA - now)
        */
    }
}