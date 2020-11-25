using System;
using System.Collections.ObjectModel;
using TimeManager.Model;
using TimeManager.Model.Events;
using TimeManager.Utilities;

namespace TimeManager.ViewModel
{
    public class EventsViewModel : NotifyPropertyChanged
    {
        private Topic _selectedTopic;
        
        public EventsViewModel()
        {
            Topics = Storage.Topics;
            TopicMover = new Mover<Topic>(Topics, SelectedTopic);
            
            Date1 = DateTime.Today;
            Date2 = DateTime.Today;
        }
        
        public Mover<Topic> TopicMover { get; set; }
        public ObservableCollection<Topic> Topics { get; set; }
        public Topic SelectedTopic
        {
            get => _selectedTopic;
            set
            {
                _selectedTopic = value;
                TopicMover.SelectedElement = value;
            }
        }


        #region new event
        
        private string _newEventDescription;
        private RelayCommand _addShortEvent;
        private RelayCommand _addLongEvent;
        private RelayCommand _addUnfinishedEvent;

        public string NewEventDescription
        {
            get => _newEventDescription;
            set
            {
                _newEventDescription = value;
                OnPropertyChanged();
            }
        }

        public DateTime Date1 { get; set; }
        public DateTime Date2 { get; set; }

        public RelayCommand AddShortEvent => _addShortEvent ?? (_addShortEvent = new RelayCommand(o =>
        {
            AddEvent(new Event(NewEventDescription, Date1));
        }, o => TopicSelected));

        public RelayCommand AddLongEvent => _addLongEvent ?? (_addLongEvent = new RelayCommand(o =>
        {
            AddEvent(new Event(NewEventDescription, new Period(Date1, Date2)));
        }, o => TopicSelected));

        public RelayCommand AddUnfinishedEvent => _addUnfinishedEvent ?? (_addUnfinishedEvent = new RelayCommand(o =>
        {
            AddEvent(new Event(NewEventDescription, new Period(Date1)));
        }, o => TopicSelected));

        private void AddEvent(Event @event)
        {
            SelectedTopic.AddEvent(@event);
            NewEventDescription = string.Empty;
        }

        private bool TopicSelected => SelectedTopic != null;

        #endregion

        #region add / remove topics

        private RelayCommand _newTopic;
        private RelayCommand _removeTopic;

        public RelayCommand NewTopic => _newTopic ?? (_newTopic = new RelayCommand(o =>
        {
            Topics.Add(new Topic("New Topic"));
        }));
        public RelayCommand RemoveTopic => _removeTopic ?? (_removeTopic = new RelayCommand(o =>
        {
            Topics.Remove(SelectedTopic);
        }, o => TopicSelected));

        #endregion
    }
}