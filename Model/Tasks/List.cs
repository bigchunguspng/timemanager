using System;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using TimeManager.Utilities;

namespace TimeManager.Model.Tasks
{
    public class List : NotifyPropertyChanged
    {
        private Task _selectedTask;
        private string _newTaskDescription;
        private DateTime _newTaskDeadline;

        public List()
        {
            Name = "New List";
            Tasks = new ObservableCollection<Task>();
            NewTaskDeadline = DateTime.Today;
        }

        [JsonProperty] public string Name { get; set; }
        [JsonProperty] public ObservableCollection<Task> Tasks { get; set; }
        
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
        [JsonIgnore] public DateTime NewTaskDeadline
        {
            get => _newTaskDeadline;
            set
            {
                _newTaskDeadline = value;
                OnPropertyChanged(nameof(NewTaskDeadline));
            }
        }

        #region commands
        
        private RelayCommand _addTask;
        private RelayCommand _removeTask;

        [JsonIgnore] public RelayCommand AddTask => _addTask ?? (_addTask = new RelayCommand(o =>
        {
            if (NewTaskDeadline > DateTime.Now)
            {
                Tasks.Add(new Task(NewTaskDescription, NewTaskDeadline));
                NewTaskDeadline = DateTime.Today;
            }
            else
                Tasks.Add(new Task(NewTaskDescription));

            NewTaskDescription = string.Empty;
        }));

        [JsonIgnore] public RelayCommand RemoveTask =>
            _removeTask ?? (_removeTask = new RelayCommand(o => Tasks.Remove(SelectedTask)));

        #endregion
    }
}