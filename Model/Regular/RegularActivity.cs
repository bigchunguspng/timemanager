using System;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using TimeManager.Model.Events;
using TimeManager.Model.Tasks;
using TimeManager.Utilities;
using TimeManager.ViewModel;

namespace TimeManager.Model.Regular
{
    public class RegularActivity : NotifyPropertyChanged
    {
        private string _description;
        private int _first;
        private int _last;

        #region constructors

        public RegularActivity()
        {
            Times = new ObservableCollection<DateTime>();
            Renamer = new Renamer();
        }
        public RegularActivity(string description) : this()
        {
            Description = description;
        }
        public RegularActivity(Task task) : this(task.Description)
        {
            Times.Add(task.Performance.StartDate());
        }
        public RegularActivity(Event @event) : this(@event.Description)
        {
            Times.Add(@event.Period.StartDate());
        }

        #endregion

        [JsonProperty] public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged();
            }
        }
        [JsonProperty] public ObservableCollection<DateTime> Times { get; set; }

        [JsonIgnore] public Renamer Renamer { get; set; }
        [JsonIgnore] public string LastTimeInfo => DateExtensions.DaysAgo(LastTime);
        [JsonIgnore] private DateTime LastTime => Times[Times.Count - 1];
        
        
        public void AddDate(DateTime date)
        {
            if (date.Date > DateTime.Today)
            {
                MainWindowViewModel.ShowInStatusBar("Can't add the future date");
                return;
            }
            if (Times.Contains(date))
            {
                MainWindowViewModel.ShowInStatusBar("This date is already included");
                return;
            }
            Times.Add(date.Date);
            for (int i = Times.Count - 1; i > 0; i--)
            {
                if (Times[i] >= Times[i - 1]) break;
                
                //swap them
                DateTime temp = Times[i - 1];
                Times[i - 1] = Times[i];
                Times[i] = temp;
            }
            OnPropertyChanged(nameof(LastTimeInfo));
        }

        #region analytics

        public int HowManyTimes(Period period)
        {
            CalculateFirstAndLastTimes(period);
            return _last - _first;
        }
        public int HowManyTimes() => Times.Count;

        public float AverageFrequency(Period period, int per = 7 /*days*/)
        {
            float result = HowManyTimes(period) / (period.Duration().Days / (float) per);
            return (float) Math.Round(result, 2);
        }
        public float AverageFrequency(int per = 7 /*days*/)
        {
            float result = HowManyTimes() / (new Period(Times[0], DateTime.Today).Duration().Days / (float) per);
            return (float) Math.Round(result, 2);
        }

        /// <summary>Присвоює "_first" індекс першого елементу після початку періоду, а "_last" - першого елементу після закінчення періоду</summary>
        private void CalculateFirstAndLastTimes(Period period)
        {
            _first = -1;
            _last  = -1;

            if (period.Start > Times[Times.Count - 1]) return;

            for (var i = 0; i < Times.Count; i++)
                if (_first < 0)
                {
                    if (Times[i] >= period.Start)
                        _first = i;
                }
                else if (Times[i] > period.End)
                    _last = i;

            if (_first < 0) _first = 0;
            if (_last < 0) _last = Times.Count;
        }

        #endregion
    }
}