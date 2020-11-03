using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace TimeManager.Model.Events
{
    public class Topic
    {
        public Topic(string name)
        {
            Name = name;
            Events = new ObservableCollection<Event>();
        }

        [JsonProperty] public string Name { get; set; }
        [JsonProperty] public ObservableCollection<Event> Events { get; set; }
    }
}