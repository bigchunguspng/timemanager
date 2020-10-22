using System.Collections.ObjectModel;

namespace TimeManager.Model.Data
{
    public class List
    {
        public List()
        {
            Name = "New List";
            Tasks = new ObservableCollection<Task>();
        }

        public string Name { get; set; }
        public ObservableCollection<Task> Tasks { get; set; }
    }
}