using System.Collections.ObjectModel;
using System.Windows;
using Newtonsoft.Json;
using TimeManager.Utilities;

namespace TimeManager.Model.Events
{
    /// <summary> Contains a group of events. </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Topic : NotifyPropertyChanged
    {
        private string _name;

        public Topic(string name)
        {
            Name = name;
            //SubTopics = new ObservableCollection<Topic>();
            Events = new ObservableCollection<Event>();
            Renamer = new Renamer();
        }

        [JsonProperty] public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }
        //[JsonProperty] public ObservableCollection<Topic> SubTopics { get; set; }
        [JsonProperty] public ObservableCollection<Event> Events { get; set; }
        [JsonProperty] public Visibility ContentVisibility { get; set; }

        //public Topic SelectedSubtopic { get; set; }
        public Event SelectedEvent { get; set; }
        public Renamer Renamer { get; set; }


        /*public void AddSubTopic(Topic topic)
        {
            SubTopics.Add(topic);
        }*/
        public void AddEvent(Event @event)
        {
            Events.Add(@event);
            
            for (int i = Events.Count - 1; i > 0; i--)
            {
                if (Events[i].Period.Start >= Events[i - 1].Period.Start) break;
                
                //swap them
                Event temp = Events[i - 1];
                Events[i - 1] = Events[i];
                Events[i] = temp;
            }
        }

        #region commands

        private RelayCommand _toggleContentVisibility;
        private RelayCommand _removeEvent;
        
        public RelayCommand ToggleContentVisibility =>
            _toggleContentVisibility ?? (_toggleContentVisibility = new RelayCommand(o =>
            {
                ContentVisibility = ContentVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
                OnPropertyChanged(nameof(ContentVisibility));
            }));

        public RelayCommand RemoveEvent => _removeEvent ?? (_removeEvent = new RelayCommand(o =>
        {
            Events.Remove(SelectedEvent);
        }, o => EventSelected));

        private bool EventSelected => SelectedEvent != null;

        #endregion
    }
}