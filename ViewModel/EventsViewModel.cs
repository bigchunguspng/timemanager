using System;
using System.Collections.ObjectModel;
using TimeManager.Model;
using TimeManager.Model.Events;
using TimeManager.Utilities;
using static TimeManager.ViewModel.MainWindowViewModel;

namespace TimeManager.ViewModel
{
    public class EventsViewModel : NotifyPropertyChanged
    {
        private Topic _selectedTopic;
        
        public EventsViewModel()
        {
            Topics = Storage.Topics;
            TopicMover = new Mover<Topic>(Topics, SelectedTopic);

            foreach (Topic topic in Topics)
                topic.EventWasSelected += SelectTopic;

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
                OnPropertyChanged();
                ShowInStatusBar("Alt+Q - move up | Alt+A - move down | Double click - rename | Middle click - minimize / maximize");
            }
        }

        private void SelectTopic(Topic topic) => SelectedTopic = topic;

        #region edit event

        private Event _eventToEdit;
        private RelayCommand _editEvent;

        private bool EditMode { get; set; }
        private Event EventToEdit
        {
            get => _eventToEdit;
            set
            {
                _eventToEdit = value;
                EditMode = value != null;
                if (value != null)
                {
                    NewEventDescription = value.Description;
                    Date1 = value.Period.Start;
                    Date2 = value.Period.IsFinished ? value.Period.End : DateTime.Today;
                }
            }
        }

        public RelayCommand EditEvent => _editEvent ?? (_editEvent = new RelayCommand(o =>
        {
            EventToEdit = SelectedTopic.SelectedEvent;
        }, o => EventSelected));

        private bool EventSelected => SelectedTopic?.SelectedEvent != null;
        
        #endregion

        #region new event
        
        private string _newEventDescription;
        private DateTime _date1;
        private DateTime _date2;
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

        public DateTime Date1
        {
            get => _date1;
            set
            {
                _date1 = value;
                OnPropertyChanged();
            }
        }
        public DateTime Date2
        {
            get => _date2;
            set
            {
                _date2 = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand AddShortEvent => _addShortEvent ?? (_addShortEvent = new RelayCommand(o =>
        {
            AddOrEdit(new Event(NewEventDescription, Date1));
        }, o => TopicSelected));

        public RelayCommand AddLongEvent => _addLongEvent ?? (_addLongEvent = new RelayCommand(o =>
        {
            AddOrEdit(new Event(NewEventDescription, new Period(Date1, Date2)));
        }, o => TopicSelected));

        public RelayCommand AddUnfinishedEvent => _addUnfinishedEvent ?? (_addUnfinishedEvent = new RelayCommand(o =>
        {
            AddOrEdit(new Event(NewEventDescription, new Period(Date1)));
        }, o => TopicSelected));

        private void AddOrEdit(Event @event)
        {
            if (EditMode)
            {
                EventToEdit.Description = @event.Description;
                EventToEdit.Period = @event.Period;
                EventToEdit.OneDay = @event.OneDay;
                EventToEdit.UpdateInfo();
                EventToEdit = null;
            }
            else
                SelectedTopic.AddEvent(@event);

            NewEventDescription = string.Empty;
            
            //Date1 = DateTime.Today;
            //Date2 = DateTime.Today;
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