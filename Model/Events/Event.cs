using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Windows;
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
            UpdateVisual(300, Year);
        }
        
        #region shit

        public void UpdateVisual(double yearLengthVisual, int year)
        {
            double yearLength = YearLength(year);

            if (Period.StartDate.Year > year)
                _start = yearLengthVisual;
            else if (Period.StartDate.Year < year)
                _start = 0;
            else
                _start = Period.StartDate.DayOfYear / yearLength * yearLengthVisual;

            double endVisual;
            if (LastDate.Year > year)
                endVisual = yearLengthVisual;
            else if (LastDate.Year < year)
                endVisual = 0;
            else
                endVisual = LastDate.DayOfYear / yearLength * yearLengthVisual;
            
            _length = OneDay && LastDate.Year == year ? 5 : endVisual - _start;
            _height = OneDay ? Period.StartDate.Year == year ? 5 : 0 : 18;
            if (OneDay) _start -= 2.5;
            UpdateVisual();
        }

        public double YearLength(int year) => DateTime.DaysInMonth(year, 2) + 337;
        public DateTime LastDate => OneDay ? Period.StartDate : Period.IsFinished ? Period.EndDate : DateTime.Today;

        private double _start;
        private double _length;
        private double _height;

        public Thickness Start => new Thickness(_start, 0, 0, 0);
        public double Length => _length;
        public double Height => _height;
        
        public double L1 => (31 + 29) / YearLength(Year) * 300;
        public double L2 => (31 + 30 + 31) / YearLength(Year) * 300;
        public double L4 => (30 + 31 + 30) / YearLength(Year) * 300;
        public double L5 => 31 / YearLength(Year) * 300;

        public void UpdateVisual()
        {
            OnPropertyChanged(nameof(L1));
            OnPropertyChanged(nameof(L2));
            OnPropertyChanged(nameof(L4));
            OnPropertyChanged(nameof(L5));
            OnPropertyChanged(nameof(Start));
            OnPropertyChanged(nameof(Length));
            OnPropertyChanged(nameof(Height));
        }

        /*int DaysInMonthesBeforeMonth(int month)
        {
            month %= 12;
            int days = 0;
            for (int i = 0; i < month; i++)
                days += _daysInMonth[i];
            return days;
        }*/

        //private int[] _daysInMonth = new int[] {31, 29, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31};

        #endregion
    }
}