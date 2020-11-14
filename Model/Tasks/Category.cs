﻿using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using Newtonsoft.Json;
using TimeManager.Utilities;
using TimeManager.ViewModel;

namespace TimeManager.Model.Tasks
{
    public class Category : NotifyPropertyChanged
    {
        private static readonly string FolderPath = $@"{MainWindowViewModel.Path}\{nameof(MainWindowViewModel.Categories)}";
        private string _name;
        private List _selectedTaskList;

        public Category(string name)
        {
            Name = name;
            TaskLists = new ObservableCollection<List>();
            ID = Hash.UniqueHash(FolderPath);
        }

        [JsonProperty] public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
        [JsonProperty] private string ID { get; set; }
        
        [JsonIgnore] public ObservableCollection<List> TaskLists { get; set; }
        [JsonIgnore] public List SelectedTaskList
        {
            get => _selectedTaskList;
            set
            {
                _selectedTaskList = value;
                OnPropertyChanged(nameof(SelectedTaskList));
            }
        }

        [JsonIgnore] private string Path => $@"{FolderPath}\{ID}.json";
        [JsonIgnore] private FileIO CategoryIO => new FileIO(Path);

        public void LoadTaskLists() => TaskLists = CategoryIO.LoadData<List>();
        public void SaveTaskLists() => CategoryIO.SaveData(TaskLists);
        public void Clear() => File.Delete(Path);

        #region deadlines indicator

        private readonly int _maxIndicatorSize = 15;
        private readonly int _maxLeftMargin = 48;
        private readonly int _maxTopMargin = 8;

        [JsonIgnore] public Thickness IndicatorMargin =>
            new Thickness(_maxLeftMargin - IndicatorSize / 2, _maxTopMargin - IndicatorSize / 2, 0, 0);

        [JsonIgnore] public float IndicatorSize => _maxIndicatorSize / (float) MinimumDaysBeforeDeadline;
        private int MinimumDaysBeforeDeadline
        {
            get
            {
                int result = Int32.MaxValue;
                foreach (var list in TaskLists)
                foreach (var task in list.Tasks)
                    if (task.HasDeadline && task.Status == TaskStatus.Unstarted)
                    {
                        int daysLeft = task.Schedule.TimeLeft().Days;
                        if (daysLeft < result) result = daysLeft;
                    }

                return Math.Abs(result) + 1;
            }
        }

        public void UpdateDeadlinesIndicator()
        {
            OnPropertyChanged(nameof(IndicatorMargin));
            OnPropertyChanged(nameof(IndicatorSize));
        }

        #endregion
    }
}