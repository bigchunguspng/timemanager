using System.Collections.Generic;

namespace TimeManager.Data
{
    public class Category
    {
        public string Name { get; set; }
        //public List<List> TaskLists { get; set; }

        public override string ToString() => Name;
    }
}