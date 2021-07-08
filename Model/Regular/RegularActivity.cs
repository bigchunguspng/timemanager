using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Newtonsoft.Json;
using TimeManager.Model.Events;
using TimeManager.Model.Tasks;
using TimeManager.Utilities;
using static TimeManager.ViewModel.MainWindowViewModel;

namespace TimeManager.Model.Regular
{
    /// <summary> Action that is performed regularly, and it's interesting to know how often. </summary>
    [JsonObject(MemberSerialization.OptIn)]
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
        public RegularActivity(bool enableRenameMode = false)
        {
            Times = new ObservableCollection<DateTime>();
            Renamer = new Renamer(enableRenameMode);
        }
        public RegularActivity(string description) : this()
        {
            Description = description;
        }
        public RegularActivity(Task task) : this(task.Description) // todo task contex menu item
        {
            Times.Add(task.Performance.StartDate);
        }
        public RegularActivity(Event @event) : this(@event.Description)
        {
            Times.Add(@event.Period.Start);
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

        public Renamer Renamer { get; set; }
        public string LastTimeInfo => LastTime.DaysAgo();
        private DateTime LastTime => Times[Times.Count - 1];

        public void AddDate(DateTime date)
        {
            if (date.Date > DateTime.Today)
            {
                ShowInStatusBar("Can't add the future date");
                return;
            }
            if (Times.Contains(date))
            {
                ShowInStatusBar("This date is already included");
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
            float result = HowManyTimes(period) / (period.Duration.Days / (float) per);
            return (float) Math.Round(result, 2);
        }
        public float AverageFrequency(int per = 7 /*days*/)
        {
            float result = HowManyTimes() / (new Period(Times[0], DateTime.Today).Duration.Days / (float) per);
            return (float) Math.Round(result, 2);
        }

        public Dictionary<int, double> IntervalDistributionChart()
        {
            Dictionary<int, double> result = new Dictionary<int, double>();
            double[] distribution = NormalizedIntervalDistribution();
            for (int i = 0; i < distribution.Length; i++)
            {
                if (result.Count == 0 && distribution[i] == 0)
                    continue;
                result.Add(i + 1, distribution[i]);
            }

            return result;
        }

        private double[] NormalizedIntervalDistribution()
        {
            int singleSegmentHeight = 8, chartHeight = 64;
            
            int[] intervals = IntervalsInDays();
            double[] distribution = new double[intervals.Max()];
            foreach (int interval in intervals)
                distribution[interval - 1] += singleSegmentHeight;

            double max = distribution.Max();

            if (max > chartHeight) //normalize if needed
            {
                double compressionRatio = chartHeight / max;
                for (int i = 0; i < distribution.Length; i++)
                {
                    distribution[i] *= compressionRatio;
                }
            }

            return distribution;
        }

        private int[] IntervalsInDays()
        {
            int times = HowManyTimes();
            int[] result = new int[times - 1];
            for (int i = 0; i < times - 1; i++)
                result[i] = (Times[i + 1] - Times[i]).Days;

            return result;
        }

        /// <summary>Присвоює "_first" індекс першого елементу після початку періоду, а "_last" - першого елементу після закінчення періоду</summary>
        private void CalculateFirstAndLastTimes(Period period)
        {
            _first = -1;
            _last  = -1;

            if (period.Start > LastTime) return;

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