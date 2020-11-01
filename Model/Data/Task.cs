using System;
using System.Collections.Generic;
using System.Windows.Input;
using Newtonsoft.Json;
using TimeManager.Utilities;
using static TimeManager.Utilities.DateExtensions;

namespace TimeManager.Model.Data
{
    public class Task : NotifyPropertyChanged
    {
        private bool _hasDeadline;
        private TaskStatus _status;
        private RelayCommand _changeTaskStatus;
        private RelayCommand _clearTask;
        private RelayCommand _setDeadline;
        private RelayCommand _clearDeadline;

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
            SetDeadline_Execute(deadline);
        }

        #endregion

        public string Description { get; set; }
        [JsonProperty] private Period Schedule { get; } //from task creation to deadline (or to the end of performance)
        [JsonProperty] private Period Performance { get; set; }
        [JsonProperty] private List<Period> Breaks { get; set; }
        [JsonProperty] private bool HasDeadline
        {
            get => _hasDeadline;
            set
            {
                _hasDeadline = value;
                NewDeadline = HasDeadline ? Schedule.End : DateTime.Today;
            }
        }

        public TaskStatus Status
        {
            get => _status;
            set
            {
                _status = value;
                UpdateButtonContent();
                UpdateTimeInfo();
                UpdateToolTip();
                OnPropertyChanged(nameof(Status));
            }
        }
        
        #region status change logic

        [JsonIgnore] public string ButtonContent
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
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
        private void UpdateButtonContent() => OnPropertyChanged(nameof(ButtonContent));

        [JsonIgnore] public RelayCommand ChangeTaskStatus 
            => _changeTaskStatus ?? (_changeTaskStatus = new RelayCommand(o => ChangeTaskStatus_Execute()));
        private void ChangeTaskStatus_Execute()
        {
            switch (Status)
            {
                case TaskStatus.Unstarted when Keyboard.IsKeyDown(Key.LeftAlt):
                    Fail();
                    break;
                case TaskStatus.Unstarted:
                    Start();
                    break;
                case TaskStatus.Performed when Keyboard.IsKeyDown(Key.LeftAlt):
                    Fail();
                    break;
                case TaskStatus.Performed:
                    Complete();
                    break;
                default:
                    if (Keyboard.IsKeyDown(Key.LeftCtrl))
                        Start();
                    else if (Status == TaskStatus.Completed)
                        Fail();
                    else
                        Complete();
                    break;
            }
        }
        private void Start()
        {
            if (Status == TaskStatus.Unstarted) 
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
            FinishTask();

            Status = TaskStatus.Completed;
        }
        private void Fail()
        {
            if (Status == TaskStatus.Unstarted && !HasDeadline) Schedule.Finish();
            FinishTask();
            
            Status = TaskStatus.Failed;
        }
        private void FinishTask()
        {
            if (Status != TaskStatus.Performed) return;
            Performance.Finish();
            if (!HasDeadline) Schedule.Finish();
        }

        [JsonIgnore] public RelayCommand ClearTask => _clearTask ?? (_clearTask = new RelayCommand(o => Clear()));
        private void Clear()
        {
            //todo: user verification dialog

            if (!HasDeadline) Schedule.End = DateTime.MinValue;
            Performance = null;
            Breaks = null;
            
            Status = TaskStatus.Unstarted;
        }

        #endregion

        #region deadline

        [JsonIgnore] public DateTime NewDeadline { get; set; }

        [JsonIgnore] public RelayCommand SetDeadline =>
            _setDeadline ?? (_setDeadline = new RelayCommand(o => SetDeadline_Execute(NewDeadline)));

        private void SetDeadline_Execute(DateTime deadline)
        {
            Schedule.End = deadline;
            HasDeadline = true;
            //NewDeadline = DateTime.Today;
            UpdateTimeInfo();
            UpdateToolTip();
        }

        [JsonIgnore] public RelayCommand ClearDeadline =>
            _clearDeadline ?? (_clearDeadline = new RelayCommand(o => ClearDeadline_Execute(), o => HasDeadline));

        private void ClearDeadline_Execute()
        {
            Schedule.End = DateTime.MinValue;
            HasDeadline = false;
            UpdateTimeInfo();
            UpdateToolTip();
        }

        #endregion

        #region time info

        [JsonIgnore] public string TimeInfo
        {
            get
            {
                switch (Status)
                {
                    case TaskStatus.Unstarted:
                        return TimeSpanToString(HasDeadline ? Schedule.TimeLeft() : Schedule.TimePassed(),
                            HasDeadline ? "left" : "ago");
                    case TaskStatus.Performed:
                        return TimeSpanToString(Performance.TimePassed() - SumOf(Breaks));
                    case TaskStatus.Completed:
                        return TimeSpanToString(Performance.Duration() - SumOf(Breaks));
                    case TaskStatus.Failed:
                        return TimeSpanToString(Schedule.Duration(), HasDeadline ? "were given" : "");
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
        public void UpdateTimeInfo() => OnPropertyChanged(nameof(TimeInfo));

        [JsonIgnore] public string ToolTipText =>
            $"Created: {DateAndTime(Schedule.Start)}{(Performance == null ? "" : $"\nPerformance: {Performance.ToString()}")}{(HasDeadline ? $"\nDeadline: {DateAndTime(Schedule.End)}" : "")}";
        private void UpdateToolTip() => OnPropertyChanged(nameof(ToolTipText));

        #endregion
    }
}