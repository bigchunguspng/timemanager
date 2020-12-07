using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Media;
using Newtonsoft.Json;
using TimeManager.Utilities;
using static TimeManager.Utilities.DateExtensions;

namespace TimeManager.Model.Tasks
{
    /// <summary> Activity that needs to be completed by a deadline or just at any time. </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Task : NotifyPropertyChanged
    {
        private bool _hasDeadline;
        private TaskStatus _status;

        #region constructors

        private Task()
        {
            Schedule = new Period();
            Status = TaskStatus.Unstarted;
            HasDeadline = false;
        }

        public Task(string description) : this()
        {
            Description = description;
        }

        public Task(string description, DateTime deadline) : this(description)
        {
            SetDeadline(deadline);
        }

        #endregion

        [JsonProperty] public string Description { get; set; }
        [JsonProperty] public Period Schedule { get; } //from task creation to deadline (or to the end of performance)
        [JsonProperty] public Period Performance { get; set; }
        [JsonProperty] private List<Period> Breaks { get; set; }
        [JsonProperty] public bool HasDeadline
        {
            get => _hasDeadline;
            set
            {
                _hasDeadline = value;
                UpdateTimeInfo();
                UpdateToolTip();
                NewDeadline = HasDeadline ? Schedule.End : DateTime.Today;
            }
        }
        [JsonProperty] public TaskStatus Status
        {
            get => _status;
            set
            {
                _status = value;
                UpdateButtonContent();
                UpdateTimeInfo();
                UpdateToolTip();
                UpdateColor();
                OnPropertyChanged();
            }
        }
        
        #region status change logic
        
        private RelayCommand _changeTaskStatus;
        private RelayCommand _clearTask;

        public string ButtonContent
        {
            get
            {
                switch (Status)
                {
                    case TaskStatus.Unstarted:
                        return "";
                    case TaskStatus.Performed:
                        return "•";
                    case TaskStatus.Completed:
                        return "✔";
                    case TaskStatus.Failed:
                        return "✘";
                    case TaskStatus.Paused:
                        return "◼";
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
        private void UpdateButtonContent() => OnPropertyChanged(nameof(ButtonContent));

        public RelayCommand ChangeTaskStatus
            => _changeTaskStatus ?? (_changeTaskStatus = new RelayCommand(o =>
            {
                if (Keyboard.IsKeyDown(Key.LeftAlt) && Status != TaskStatus.Completed && Status != TaskStatus.Failed)
                    Fail();
                else if (Keyboard.IsKeyDown(Key.LeftCtrl) && Status != TaskStatus.Unstarted &&
                         Status != TaskStatus.Performed)
                    Start();
                else if (Keyboard.IsKeyDown(Key.LeftShift) && Status != TaskStatus.Unstarted &&
                         Status != TaskStatus.Paused)
                    Pause();
                else
                    switch (Status)
                    {
                        case TaskStatus.Unstarted:
                            Start();
                            break;
                        case TaskStatus.Performed:
                            Complete();
                            break;
                        case TaskStatus.Completed:
                            Fail();
                            break;
                        case TaskStatus.Failed:
                            Complete();
                            break;
                        case TaskStatus.Paused:
                            Complete();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
            }));
        
        private void Start()
        {
            if (Performance == null)
                Performance = new Period();
            else
            {
                if (Breaks == null) Breaks = new List<Period>();
                Breaks.Add(new Period(Performance.End, DateTime.Now));
                Performance.End = DateTime.MinValue;
            }
            
            Status = TaskStatus.Performed;
        }
        private void Complete()
        {
            StopPerformance();

            Status = TaskStatus.Completed;
        }
        private void Fail()
        {
            if (Status == TaskStatus.Unstarted && !HasDeadline) Schedule.Finish();
            StopPerformance();
            
            Status = TaskStatus.Failed;
        }
        private void Pause()
        {
            StopPerformance();
            
            Status = TaskStatus.Paused;
        }
        private void StopPerformance()
        {
            if (Status != TaskStatus.Performed) return;
            Performance.Finish();
            if (!HasDeadline) Schedule.Finish();
        }

        public RelayCommand ClearTask => _clearTask ?? (_clearTask = new RelayCommand(o =>
        {
            //todo: user verification dialog

            if (!HasDeadline) Schedule.End = DateTime.MinValue;
            Performance = null;
            Breaks = null;

            Status = TaskStatus.Unstarted;
        }));

        #endregion

        #region deadline
        
        private RelayCommand _addDeadline;
        private RelayCommand _clearDeadline;

        public DateTime NewDeadline { get; set; }

        public RelayCommand AddDeadline =>
            _addDeadline ?? (_addDeadline = new RelayCommand(o => SetDeadline(NewDeadline)));

        private void SetDeadline(DateTime deadline)
        {
            Schedule.End = deadline;
            HasDeadline = true;
        }

        public RelayCommand ClearDeadline =>
            _clearDeadline ?? (_clearDeadline = new RelayCommand(o =>
            {
                Schedule.End = DateTime.MinValue;
                HasDeadline = false;
            }, o => HasDeadline));

        #endregion

        #region time info

        public string TimeInfo
        {
            get
            {
                switch (Status)
                {
                    case TaskStatus.Unstarted:
                        return TimeSpanToString(HasDeadline ? Schedule.TimeLeft : Schedule.TimePassed,
                            HasDeadline ? "left" : "ago");
                    case TaskStatus.Performed:
                        return TimeSpanToString(Performance.TimePassed - SumOf(Breaks));
                    case TaskStatus.Completed:
                        return TimeSpanToString(Performance.Duration - SumOf(Breaks)); // bug System.Windows.Data Error: 17 : Cannot get 'TimeInfo' value
                    case TaskStatus.Failed:
                        return TimeSpanToString(Schedule.Duration, HasDeadline ? "were given" : "");
                    case TaskStatus.Paused:
                        return TimeSpanToString(Performance.Duration - SumOf(Breaks));
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
        public void UpdateTimeInfo() => OnPropertyChanged(nameof(TimeInfo));

        public string ToolTipText =>
            $"Created: {Schedule.Start.DateAndTime()}" +
            $"{(Performance == null ? $"{(!HasDeadline && Status != TaskStatus.Performed && Status != TaskStatus.Unstarted ? $"\n{Status}: {Schedule.End.DateAndTime()}" : "")}" : $"\nPerformance: {Performance}")}" +
            $"{(HasDeadline ? $"\nDeadline: {Schedule.End.DateAndTime()}" : "")}";
        private void UpdateToolTip() => OnPropertyChanged(nameof(ToolTipText));

        #endregion

        #region color

        public Brush Color =>
            Status == TaskStatus.Completed || Status == TaskStatus.Failed
                ? new SolidColorBrush(Colors.Silver)
                : new SolidColorBrush(Colors.Black);

        private void UpdateColor() => OnPropertyChanged(nameof(Color));

        #endregion
    }
}