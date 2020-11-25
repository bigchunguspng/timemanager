using System;
using System.Collections.ObjectModel;
using TimeManager.Model;
using TimeManager.Model.Events;
using TimeManager.Utilities;

namespace TimeManager.ViewModel
{
    public class EventsViewModel
    {
        public EventsViewModel()
        {
            Topics = Storage.Topics;
            
            Date1 = DateTime.Today;
            Date2 = DateTime.Today;
        }
        
        public ObservableCollection<Topic> Topics { get; set; }
        public Topic SelectedTopic { get; set; }

        #region new event

        private RelayCommand _addShortEvent;
        private RelayCommand _addLongEvent;
        private RelayCommand _addUnfinishedEvent;

        public string NewEventDescription { get; set; }
        public DateTime Date1 { get; set; }
        public DateTime Date2 { get; set; }

        public RelayCommand AddShortEvent => _addShortEvent ?? (_addShortEvent = new RelayCommand(o =>
        {
            SelectedTopic.Events.Add(new Event(NewEventDescription, Date1));
        }, o => TopicSelected));

        public RelayCommand AddLongEvent => _addLongEvent ?? (_addLongEvent = new RelayCommand(o =>
        {
            SelectedTopic.Events.Add(new Event(NewEventDescription, new Period(Date1, Date2)));
        }, o => TopicSelected));

        public RelayCommand AddUnfinishedEvent => _addUnfinishedEvent ?? (_addUnfinishedEvent = new RelayCommand(o =>
        {
            SelectedTopic.Events.Add(new Event(NewEventDescription, new Period(Date1)));
        }, o => TopicSelected));

        private bool TopicSelected => SelectedTopic != null;

        #endregion

        #region topics

        private RelayCommand _moveUp;
        private RelayCommand _moveDown;
        private RelayCommand _newTopic;
        private RelayCommand _removeTopic;
        
        public RelayCommand MoveUp => _moveUp ?? (_moveUp = new RelayCommand(o =>
        {
            int index = SelectedTopicIndex;
            Topics.Move(index, index - 1);
        }, o => TopicSelected && TopicNotFirst));

        public RelayCommand MoveDown => _moveDown ?? (_moveDown = new RelayCommand(o =>
        {
            int index = SelectedTopicIndex;
            Topics.Move(index, index + 1);
        }, o => TopicSelected && TopicNotLast));

        public RelayCommand NewTopic => _newTopic ?? (_newTopic = new RelayCommand(o =>
        {
            Topics.Add(new Topic("New Topic"));
        }));
        public RelayCommand RemoveTopic => _removeTopic ?? (_removeTopic = new RelayCommand(o =>
        {
            Topics.Remove(SelectedTopic);
        }, o => TopicSelected));
        
        private bool TopicNotFirst => SelectedTopicIndex > 0;
        private bool TopicNotLast => SelectedTopicIndex < Topics.Count - 1;
        private int SelectedTopicIndex => Topics.IndexOf(SelectedTopic);
        

        #endregion
    }
}