using System.Collections.ObjectModel;
using Newtonsoft.Json;
using TimeManager.Utilities;
using static TimeManager.ViewModel.MainWindowViewModel;

namespace TimeManager.Model.Events
{
    /// <summary> Contains a group of events. </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Topic : Collapsible
    {
        private string _name;
        private Event _selectedEvent;

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

        //public Topic SelectedSubtopic { get; set; }
        public Event SelectedEvent
        {
            get => _selectedEvent;
            set
            {
                _selectedEvent = value;
                OnPropertyChanged();
                //EventsViewModel.SelectedTopic = this;
                ShowInStatusBar("Double click - rename");
            }
        }

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

        private RelayCommand _removeEvent;
        
        public RelayCommand RemoveEvent => _removeEvent ?? (_removeEvent = new RelayCommand(o =>
        {
            Events.Remove(SelectedEvent);
        }, o => EventSelected));

        private bool EventSelected => SelectedEvent != null;

        #endregion
    }
}