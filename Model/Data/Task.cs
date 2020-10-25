using System;
using System.Collections.Generic;
using System.Windows.Input;
using Newtonsoft.Json;
using TimeManager.Utilities;

namespace TimeManager.Model.Data
{
    public class Task : NotifyPropertyChanged
    {
        private TaskStatus _status;
        private string _buttonContent;
        private RelayCommand _changeTaskStatus;
        private RelayCommand _clearTask;

        #region constructors

        public Task()
        {
            Schedule.Start = DateTime.Now;
            Status = TaskStatus.Unstarted;
            ButtonContent = "";
        }

        public Task(string description) : this()
        {
            Description = description;
        }

        public Task(DateTime deadline) : this()
        {
            Schedule.End = deadline;
            Schedule.Finished = true;
            HasDeadline = true;
        }

        #endregion

        public string Description { get; set; }
        public Period Schedule { get; } = new Period(); //from task creation to deadline (or to the end of performance)
        public Period Performance { get; set; }
        public List<Period> Breaks { get; set; }
        public bool HasDeadline { get; }
        public TaskStatus Status
        {
            get => _status;
            private set
            {
                _status = value;
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

        [JsonIgnore] public RelayCommand ChangeTaskStatus 
            => _changeTaskStatus ?? (_changeTaskStatus = new RelayCommand(o => ChangeTaskStatus_Execute()));
        private void ChangeTaskStatus_Execute()
        {
            if (Status == TaskStatus.Unstarted)
            {
                if (Keyboard.IsKeyDown(Key.LeftAlt))
                {
                    Fail();
                }
                else
                {
                    Start();
                }
            }
            else if (Status == TaskStatus.Performed)
            {
                if (Keyboard.IsKeyDown(Key.LeftAlt))
                {
                    Fail();
                }
                else
                {
                    Complete();
                }
            }
            else
            {
                if (Keyboard.IsKeyDown(Key.LeftCtrl))
                {
                    Start();
                }
                else
                {
                    if (Status == TaskStatus.Completed)
                    {
                        Fail();
                    }
                    else
                    {
                        Complete();
                    }
                }
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
            ButtonContent = "•";
        }
        private void Complete()
        {
            FinishTask();

            Status = TaskStatus.Completed;
            ButtonContent = "✔";
        }
        private void Fail()
        {
            if (Status == TaskStatus.Unstarted && !HasDeadline) Schedule.Finish();
            FinishTask();
            
            Status = TaskStatus.Failed;
            ButtonContent = "✘";
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
            Status = TaskStatus.Unstarted;
            
            //todo: user verification dialog
            
            if (!HasDeadline) Schedule.End = DateTime.MinValue;
            Performance = null;
            Breaks = null;
            ButtonContent = "";
        }

        #endregion
    }
}