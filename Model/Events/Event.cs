using System;
using Newtonsoft.Json;
using TimeManager.Model.Tasks;
using TimeManager.Utilities;

namespace TimeManager.Model.Events
{
    /// <summary> Chronological event that continues one or more days and can take place in past, present or future. </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Event : NotifyPropertyChanged
    {
        private string _description;

        #region constructors

        public Event()
        {
            Renamer = new Renamer();
        }
        /// <summary> Creates event that continues more than one day. </summary>
        public Event(string description, Period period) : this()
        {
            Description = description;
            Period = period;
        }
        /// <summary> Creates event that continues only one day. </summary>
        public Event(string description, DateTime date) : this(description, new Period(date))
        {
            OneDay = true;
        }

        public Event(Task task) : this(task.Description, task.Performance)
        {
            if (task.Performance.Start == task.Performance.End) OneDay = true;
        }

        //todo перевірка на (дата 2 > дати1)
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

        [JsonProperty] public Period Period { get; set; }
        [JsonProperty] public bool OneDay { get; set; }

        public Renamer Renamer { get; set; }

        public string TimeInfo => OneDay
            ? Period.Start.DateOnly()
            : Period.ToString(true);
        public string DurationInfo => OneDay
            ? Period.Start.DaysAgo()
            : DateExtensions.TimeSpanToString(Period.IsFinished ? Period.Duration + TimeSpan.FromDays(1) : Period.Duration);

        public void UpdateInfo()
        {
            OnPropertyChanged(nameof(Description));
            OnPropertyChanged(nameof(TimeInfo));
            OnPropertyChanged(nameof(DurationInfo));
        }
        
        
        /*public TimeSpan TimeFromStart => Period.TimePassed();
        public TimeSpan TimeFromEnd => - Period.TimeLeft();
        public TimeSpan Duration => Period.Duration();*/
    }
}