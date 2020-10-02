using System.Collections.Generic;

namespace TimeManager.Data
{
    public class List
    {
        public string Name { get; set; }
        public List<Task> Tasks { get; set; }
    }
}