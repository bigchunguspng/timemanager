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
        private string _name;
        private Task _selectedTask;
        private string _newTaskDescription;
        private DateTime _newTaskDeadline;

        public List()
        {
            Name = "New List";
            Tasks = new ObservableCollection<Task>();
            NewTaskDeadline = DateTime.Today;
            Renamer = new Renamer();
        }

        [JsonProperty] public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }
        [JsonProperty] public ObservableCollection<Task> Tasks { get; set; }
        
        [JsonProperty] public Visibility TasksVisibility { get; set; }
        [JsonIgnore] public string TasksInfo
        {
            get
            {
                if (TasksVisibility == Visibility.Visible)
                    return "";
                else
                {
                    string signs = "▢◉✔✘◼";
                    int[] numberOfTasksWithStatus = new int[signs.Length];
                    foreach (var task in Tasks)
                        numberOfTasksWithStatus[(int) task.Status]++;

                    string result = "";
                    for (var i = 0; i < numberOfTasksWithStatus.Length; i++)
                        if (numberOfTasksWithStatus[i] > 0)
                            result = $"{result} {numberOfTasksWithStatus[i]}{signs[i]}";

                    return result;
                }
            }
        }

        [JsonIgnore] public Renamer Renamer { get; set; }
        [JsonIgnore] public Task SelectedTask
        {
            get => _selectedTask;
            set
            {
                _selectedTask = value;
                OnPropertyChanged(nameof(SelectedTask));
                MainWindowViewModel.ShowInStatusBar("Delete - delete task");
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
        
        private RelayCommand _toggleTasksVisibility;
        private RelayCommand _addTask;
        private RelayCommand _removeTask;

        [JsonIgnore] public RelayCommand ToggleTasksVisibility =>
            _toggleTasksVisibility ?? (_toggleTasksVisibility = new RelayCommand(o =>
            {
                TasksVisibility = TasksVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
                OnPropertyChanged(nameof(TasksVisibility));
                OnPropertyChanged(nameof(TasksInfo));
                UpdateStatusBar();
            }));
        public void UpdateStatusBar()
        {
            MainWindowViewModel.ShowInStatusBar(
                "Alt+Q - move up | Alt+A - move down | Middle click - " +
                (TasksVisibility == Visibility.Visible
                    ? "minimize"
                    : "maximize"));
        }

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