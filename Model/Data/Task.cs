using System;
using TimeManager.Utilities;

namespace TimeManager.Model.Data
{
    public class Task : NotifyPropertyChanged
    {
        private bool _started;
        private bool _finished;
        private bool _succeeded;

        public Task()
        {
            Schedule.Start = DateTime.Now;
        }
        public Task(string description)
        {
            Schedule.Start = DateTime.Now;
            Description = description;
        }

        public Task(DateTime deadline)
        {
            Schedule.End = deadline;
            Schedule.Finished = true;
            HasDeadline = true;
        }

        public string Description { get; set; }

        public Period Schedule { get; } = new Period(); //from task creation to deadline (or to the end of performance)
        public Period Performance { get; } = new Period();

        public bool HasDeadline { get; }

        public bool Started //point
        {
            get => _started;
            set
            {
                _started = value;
                if (value) Performance.Start = DateTime.Now;
                OnPropertyChanged(nameof(Started));
            }
        }
        public bool Finished //tick
        {
            get => _finished;
            set
            {
                _finished = value;
                Performance.End = DateTime.Now;
                if (!HasDeadline) Schedule.End = DateTime.Now; //if deadline is declared
                Succeeded = value;
                OnPropertyChanged(nameof(Finished));
            }
        }
        public bool Succeeded //cross if not
        {
            get => _succeeded;
            set
            {
                _succeeded = value;
                OnPropertyChanged(nameof(Succeeded));
            }
        }
    }
}