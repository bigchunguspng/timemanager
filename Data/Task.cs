using System;

namespace TimeManager.Data
{
    public class Task
    {
        private bool _started;
        private bool _finished;
        
        public string Description { get; set; }
        public DateTime Creation { get; }
        public DateTime Deadline { get; set; }
        public DateTime Start { get; private set; }
        public DateTime End { get; private set; }
        public bool Started
        {
            get => _started;
            set
            {
                _started = value;
                Start = DateTime.Now;
            }
        }
        public bool Finished
        {
            get => _finished;
            set
            {
                _finished = value;
                End = DateTime.Now;
                Succeeded = !Succeeded;
            }
        }
        public bool Succeeded { get; set; }
        
        public Task()
        {
            Creation = DateTime.Now;
        }
    }
}