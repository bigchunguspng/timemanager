using System;

namespace TimeManager.Model.Data
{
    public class Task
    {
        private bool _started;
        private bool _finished;
        
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
                Performance.Start = DateTime.Now;
            }
        }
        public bool Finished //tick
        {
            get => _finished;
            set
            {
                _finished = true;
                Performance.End = DateTime.Now;
                if (!HasDeadline) Schedule.End = DateTime.Now; //if deadline is declared
                Succeeded = value;
            }
        }
        public bool Succeeded { get; set; } //cross if not
        
        public Task()
        {
            Schedule.Start = DateTime.Now;
        }

        public Task(DateTime deadline)
        {
            Schedule.End = deadline;
            Schedule.Finished = true;
            HasDeadline = true;
        }
    }
}