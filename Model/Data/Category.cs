using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TimeManager.Properties;
using TimeManager.Utilities;

namespace TimeManager.Model.Data
{
    public class Category : INotifyPropertyChanged
    {
        private string _name;

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

        public void TestTaskLists() //todo ability to create lists from UI
        {
            var l1 = new List {Name = "Project #1"};
            l1.TestTasks();
            TaskLists.Add(l1);
            var l2 = new List {Name = "Project #2"};
            l2.TestTasks();
            TaskLists.Add(l2);
            TaskLists.Add(new List{Name = "Project #??"});
        }
        
        #region stuff
        public override string ToString() => Name;

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
        
    }
}