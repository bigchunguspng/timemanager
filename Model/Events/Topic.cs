using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace TimeManager.Model.Events
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Topic
    {
        public Topic(string name)
        {
            Name = name;
            SubTopics = new ObservableCollection<Topic>();
            Events = new ObservableCollection<IEvent>();
        }

        [JsonProperty] public string Name { get; set; }
        [JsonProperty] public ObservableCollection<Topic> SubTopics { get; set; }
        [JsonProperty] public ObservableCollection<IEvent> Events { get; set; }


        public void AddSubTopic(Topic topic)
        {
            SubTopics.Add(topic);
        }
        public void AddLongEvent(LongEvent longEvent)
        {
            Events.Add(longEvent);
        }
        public void AddShortEvent(ShortEvent shortEvent)
        {
            Events.Add(shortEvent);
        }
    }
}