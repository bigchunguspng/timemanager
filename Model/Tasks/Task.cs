using System;
using System.Collections.Generic;
using System.Windows.Input;
using Newtonsoft.Json;
using TimeManager.Utilities;
using static TimeManager.Utilities.DateExtensions;

namespace TimeManager.Model.Tasks
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
        [JsonProperty] public Period Schedule { get; } //from task creation to deadline (or to the end of performance)
        [JsonProperty] public Period Performance { get; set; }
        [JsonProperty] private List<Period> Breaks { get; set; }
        [JsonProperty] public bool HasDeadline
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
                    case TaskStatus.Paused:
                        return "⏸";
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
            if (Keyboard.IsKeyDown(Key.LeftAlt) && Status != TaskStatus.Completed && Status != TaskStatus.Failed)
                Fail();
            else if (Keyboard.IsKeyDown(Key.LeftCtrl) && Status != TaskStatus.Unstarted && Status != TaskStatus.Performed)
                Start();
            else if (Keyboard.IsKeyDown(Key.LeftShift) && Status != TaskStatus.Unstarted && Status != TaskStatus.Paused)
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
        private void Pause()
        {
            FinishTask();
            
            Status = TaskStatus.Paused;
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
            //NewDeadline = DateTime.AddDate;
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
                    case TaskStatus.Paused:
                        return TimeSpanToString(Performance.Duration() - SumOf(Breaks));
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