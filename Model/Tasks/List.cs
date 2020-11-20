using System;
using System.Collections.ObjectModel;
using System.Windows;
using Newtonsoft.Json;
using TimeManager.Utilities;
using TimeManager.ViewModel;

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
        
        [JsonProperty] public Visibility TasksVisibility { get; set; }
        [JsonIgnore] public string TasksInfo
        {
            get
            {
                if (TasksVisibility == Visibility.Visible)
                    return string.Empty;
                else
                {
                    string signs = "▢◉✔✘◼";
                    int[] numberOfTasksWithStatus = new int[signs.Length];
                    foreach (var task in Tasks)
                        numberOfTasksWithStatus[(int) task.Status]++;

                    string result = string.Empty;
                    for (var i = 0; i < numberOfTasksWithStatus.Length; i++)
                        if (numberOfTasksWithStatus[i] > 0)
                            result = $"{result} {numberOfTasksWithStatus[i]}{signs[i]}";

                    return result;
                }
            }
        }

        [JsonIgnore] public Task SelectedTask
        {
            get => _selectedTask;
            set
            {
                _selectedTask = value;
                OnPropertyChanged(nameof(SelectedTask));
                MainWindowViewModel.ShowInStatusBar("Delete - remove task");
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
        
        private RelayCommand _changeTasksVisibility;
        private RelayCommand _addTask;
        private RelayCommand _removeTask;

        [JsonIgnore] public RelayCommand ChangeTasksVisibility =>
            _changeTasksVisibility ?? (_changeTasksVisibility = new RelayCommand(o =>
            {
                TasksVisibility = TasksVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
                OnPropertyChanged(nameof(TasksVisibility));
                OnPropertyChanged(nameof(TasksInfo));
            }));

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