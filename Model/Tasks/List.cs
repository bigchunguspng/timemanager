using System;
using System.Collections.ObjectModel;
using System.Windows;
using Newtonsoft.Json;
using TimeManager.Utilities;
using static TimeManager.ViewModel.MainWindowViewModel;

namespace TimeManager.Model.Tasks
{
    /// <summary> List of tasks; to‐do list. </summary>
    [JsonObject(MemberSerialization.OptIn)]
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
        
        [JsonProperty] public Visibility ContentVisibility { get; set; }
        public string TasksInfo
        {
            get
            {
                if (ContentVisibility == Visibility.Visible)
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

        public Renamer Renamer { get; set; }
        public Task SelectedTask
        {
            get => _selectedTask;
            set
            {
                _selectedTask = value;
                OnPropertyChanged();
                ShowInStatusBar("Delete - delete task");
            }
        }
        public string NewTaskDescription
        {
            get => _newTaskDescription;
            set
            {
                _newTaskDescription = value;
                OnPropertyChanged();
            }
        }
        public DateTime NewTaskDeadline
        {
            get => _newTaskDeadline;
            set
            {
                _newTaskDeadline = value;
                OnPropertyChanged();
            }
        }

        #region commands
        
        private RelayCommand _toggleContentVisibility;
        private RelayCommand _addTask;
        private RelayCommand _removeTask;

        public RelayCommand ToggleContentVisibility =>
            _toggleContentVisibility ?? (_toggleContentVisibility = new RelayCommand(o =>
            {
                ContentVisibility = ContentVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
                OnPropertyChanged(nameof(ContentVisibility));
                OnPropertyChanged(nameof(TasksInfo));
                UpdateStatusBar();
            }));
        public void UpdateStatusBar()
        {
            ShowInStatusBar(
                "Alt+Q - move up | Alt+A - move down | Middle click - " +
                (ContentVisibility == Visibility.Visible
                    ? "minimize"
                    : "maximize"));
        }

        public RelayCommand AddTask => _addTask ?? (_addTask = new RelayCommand(o =>
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

        public RelayCommand RemoveTask =>
            _removeTask ?? (_removeTask = new RelayCommand(o => Tasks.Remove(SelectedTask)));

        #endregion
    }
}