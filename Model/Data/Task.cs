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
        private TaskStatus _status;
        private string _buttonContent;
        private RelayCommand _changeTaskStatus;
        private RelayCommand _clearTask;
        private RelayCommand _setDeadline;

        #region constructors

        private Task()
        {
            Schedule = new Period();
            Status = TaskStatus.Unstarted;
        }

        public Task(string description) : this()
        {
            Description = description;
        }

        public Task(string description, DateTime deadline) : this(description)
        {
            Schedule.End = deadline;
            HasDeadline = true;
        }

        #endregion

        public string Description { get; set; }
        [JsonProperty] private Period Schedule { get; } //from task creation to deadline (or to the end of performance)
        [JsonProperty] private Period Performance { get; set; }
        [JsonProperty] private List<Period> Breaks { get; set; }
        [JsonProperty] private bool HasDeadline { get; set; }
        public TaskStatus Status
        {
            get => _status;
            set
            {
                _status = value;
                UpdateButtonContent();
                UpdateToolTip();
                OnPropertyChanged(nameof(Status));
            }
        }
        
        #region status change logic

        [JsonIgnore] public string ButtonContent
        {
            get => _buttonContent;
            set
            {
                _buttonContent = value;
                OnPropertyChanged(nameof(ButtonContent));
            }
        }
        private void UpdateButtonContent()
        {
            switch (Status)
            {
                case TaskStatus.Unstarted:
                    ButtonContent = "";
                    break;
                case TaskStatus.Performed:
                    ButtonContent = "•";
                    break;
                case TaskStatus.Completed:
                    ButtonContent = "✔";
                    break;
                case TaskStatus.Failed:
                    ButtonContent = "✘";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

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

        [JsonIgnore] public RelayCommand ClearDeadline =>
            _setDeadline ?? (_setDeadline = new RelayCommand(o => ClearDeadline_Execute(), o => HasDeadline));

        private void ClearDeadline_Execute()
        {
            Schedule.End = DateTime.MinValue;
            HasDeadline = false;
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
                        return TimeSpanToString(Schedule.Duration());
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public void UpdateTimeInfo() => OnPropertyChanged(nameof(TimeInfo));

        [JsonIgnore] public string ToolTipText =>
            $"Created: {DateAndTime(Schedule.Start)}{(Performance == null ? "" : $"\nPerformance: {Performance.ToString()}")}";

        private void UpdateToolTip() => OnPropertyChanged(nameof(ToolTipText));

        #endregion
    }
}