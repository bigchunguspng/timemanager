using System;
using Newtonsoft.Json;
using TimeManager.Model.Tasks;
using TimeManager.Utilities;
using static TimeManager.Model.Storage;

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
            Visual = new EventVisual(this);
        }
        /// <summary> Creates event that continues more than one day. </summary>
        public Event(string description, Period period) : this()
        {
            Description = description;
            Period = period;
            UpdateInfo();
        }
        /// <summary> Creates event that continues only one day. </summary>
        public Event(string description, DateTime date) : this(description, new Period(date))
        {
            OneDay = true;
            UpdateInfo();
        }

        public Event(Task task) : this(task.Description, task.Performance)
        {
            if (task.Performance.Start == task.Performance.End) OneDay = true;
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

        [JsonProperty] public Period Period { get; set; }
        [JsonProperty] public bool OneDay { get; set; }

        public Renamer Renamer { get; set; }
        public EventVisual Visual { get; set; }
        public EventVisual[] Visuals { get; set; }

        public string TimeInfo => OneDay
            ? Period.Start.DateOnly()
            : Period.ToString(true);
        public string DurationInfo => OneDay
            ? Period.Start.DaysAgo()
            : DateExtensions.TimeSpanToString(Period.IsFinished ? Period.Duration + TimeSpan.FromDays(1) : Period.Duration);
        
        public DateTime LastDate => OneDay ? Period.StartDate : Period.IsFinished ? Period.EndDate : DateTime.Today;

        public void UpdateInfo()
        {
            OnPropertyChanged(nameof(Description));
            OnPropertyChanged(nameof(TimeInfo));
            OnPropertyChanged(nameof(DurationInfo));
            Visual.Update(300, Year, true);
            UpdateVisuals();
        }

        private void UpdateVisuals()
        {
            int yearCount = LastDate.Year - Period.StartDate.Year + 1;
            int year = Period.StartDate.Year;
            Visuals = new EventVisual[yearCount];
            for (var i = 0; i < Visuals.Length; i++)
            {
                Visuals[i] = new EventVisual(this);
                Visuals[i].Update(300, year);
                year++;
            }
        }
    }
}