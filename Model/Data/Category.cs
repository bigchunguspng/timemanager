using System.Collections.ObjectModel;
using TimeManager.Utilities;

namespace TimeManager.Model.Data
{
    public class Category : NotifyPropertyChanged
    {
        private string _name;
        private List _selectedTaskList;

        public Category(string name) => Name = name;

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
        public ObservableCollection<List> TaskLists { get; set; } = new ObservableCollection<List>();
        public List SelectedTaskList
        {
            get => _selectedTaskList;
            set
            {
                _selectedTaskList = value;
                OnPropertyChanged(nameof(SelectedTaskList));
            }
        }

        public void TestTaskLists() //todo replace with json loader
        {
            var l1 = new List {Name = "Project #1"};
            l1.TestTasks();
            TaskLists.Add(l1);
            var l2 = new List {Name = "Project #2"};
            l2.TestTasks();
            TaskLists.Add(l2);
            TaskLists.Add(new List{Name = "Project #??"});
        }
        
        public override string ToString() => Name;
    }
}