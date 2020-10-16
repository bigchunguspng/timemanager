using System.Collections.ObjectModel;

namespace TimeManager.Model.Data
{
    public class List
    {
        public List() => Name = "New List";

        public string Name { get; set; }
        public ObservableCollection<Task> Tasks { get; set; } = new ObservableCollection<Task>();
    }
}