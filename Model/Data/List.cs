using System;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using TimeManager.Utilities;

namespace TimeManager.Model.Data
{
    public class List : NotifyPropertyChanged
    {
        private Task _selectedTask;
        private string _newTaskDescription;
        private RelayCommand _addTask;
        private RelayCommand _removeTask;

        public List()
        {
            Name = "New List";
            Tasks = new ObservableCollection<Task>();
        }

        public string Name { get; set; }
        public ObservableCollection<Task> Tasks { get; set; }
        
        [JsonIgnore] public Task SelectedTask
        {
            get => _selectedTask;
            set
            {
                _selectedTask = value;
                OnPropertyChanged(nameof(SelectedTask));
            }
        }
        [JsonIgnore] public string NewTaskDescription
        {
            get => _newTaskDescription;
            set
            {
                _newTaskDescription = value;
                OnPropertyChanged(nameof(NewTaskDescription));
            }
        }

        #region commands
        
        [JsonIgnore] public RelayCommand AddTask => _addTask ?? (_addTask = new RelayCommand(NewTask_Execute));
        private void NewTask_Execute(object o)
        {
            Tasks.Add(new Task(NewTaskDescription));
            NewTaskDescription = String.Empty;
        }
        
        [JsonIgnore] public RelayCommand RemoveTask =>
            _removeTask ?? (_removeTask = new RelayCommand(o => Tasks.Remove(SelectedTask)));

        #endregion
    }
}