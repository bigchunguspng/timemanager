﻿using System.Collections.ObjectModel;
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
        public void AddEvent(Event @event) => Events.Add(@event); //todo sort

        #region commands

        private RelayCommand _toggleContentVisibility;
        
        public RelayCommand ToggleContentVisibility =>
            _toggleContentVisibility ?? (_toggleContentVisibility = new RelayCommand(o =>
            {
                ContentVisibility = ContentVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
                OnPropertyChanged(nameof(ContentVisibility));
            }));

        #endregion
    }
}