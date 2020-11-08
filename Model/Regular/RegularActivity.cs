using System;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using TimeManager.Model.Events;
using TimeManager.Model.Tasks;
using TimeManager.Utilities;

namespace TimeManager.Model.Regular
{
    public class RegularActivity : NotifyPropertyChanged
    {
        private int _first;
        private int _last;

        #region constructors

        public RegularActivity()
        {
            Times = new ObservableCollection<DateTime>();
        }
        public RegularActivity(string description) : this()
        {
            Description = description;
        }
        public RegularActivity(Task task) : this(task.Description)
        {
            Times.Add(task.Performance.ToDateTime());
        }
        public RegularActivity(Event @event) : this(@event.Description)
        {
            Times.Add(@event.Period.ToDateTime());
        }

        #endregion
        
        [JsonProperty] public string Description { get; set; }
        [JsonProperty] public ObservableCollection<DateTime> Times { get; set; }

        [JsonIgnore] public string LastTimeInfo => DateExtensions.DaysAgo(LastTime);
        private DateTime LastTime => Times[Times.Count - 1];
        
        
        public void AddDate(DateTime date)
        {
            if (date.Date > DateTime.Today)
                return; //todo show result in tipbar
            if (Times.Contains(date))
                return; //todo show result in tipbar
            Times.Add(date.Date);
            for (int i = Times.Count - 1; i > 0; i--)
            {
                if (Times[i] >= Times[i - 1])
                    break;
                //swap them
                DateTime temp = Times[i - 1];
                Times[i - 1] = Times[i];
                Times[i] = temp;
            }
            OnPropertyChanged(nameof(LastTimeInfo));
        }

        //private int DaysSinceLastTime() => (DateTime.Today - LastTime.Date).Days;

        public int HowManyTimes(Period period)
        {
            CalculateFirstAndLastTimes(period);
            return _last - _first;
        }

        public float AverageFrequency(Period period, int per = 7 /*days*/)
        {
            CalculateFirstAndLastTimes(period);
            float result = (_last - _first) / (period.Duration().Days / (float) per);
            return (float) Math.Round(result, 2);
        }

        /// <summary>Присвоює "First" індекс першого елементу після початку періоду, а "Last" - першого елементу після закінчення періоду</summary>
        private void CalculateFirstAndLastTimes(Period period)
        {
            _first = -1;
            _last  = -1;

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
    }
}