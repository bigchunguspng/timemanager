using System;
using Newtonsoft.Json;

namespace TimeManager.Model.Events
{
    [JsonObject(MemberSerialization.OptIn)]
    public interface IEvent
    {
        [JsonProperty] string Description { get; set; }
        
        TimeSpan TimeFromStart { get; }
        TimeSpan TimeFromEnd { get; }
        TimeSpan Duration { get; }
    }
}