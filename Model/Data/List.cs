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
        private DateTime _deadline;

        public List()
        {
            Name = "New List";
            Tasks = new ObservableCollection<Task>();
            Deadline = DateTime.Today;
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
        [JsonIgnore] public DateTime Deadline
        {
            get => _deadline;
            set
            {
                _deadline = value;
                OnPropertyChanged(nameof(Deadline));
            }
        }

        #region commands

        [JsonIgnore] public RelayCommand AddTask => _addTask ?? (_addTask = new RelayCommand(AddTask_Execute));

        private void AddTask_Execute(object o)
        {
            if (Deadline > DateTime.Now)
            {
                Tasks.Add(new Task(NewTaskDescription, Deadline));
                Deadline = DateTime.Today;
            }
            else
                Tasks.Add(new Task(NewTaskDescription));
            NewTaskDescription = string.Empty;
        }

        [JsonIgnore] public RelayCommand RemoveTask =>
            _removeTask ?? (_removeTask = new RelayCommand(o => Tasks.Remove(SelectedTask)));

        #endregion
    }
}