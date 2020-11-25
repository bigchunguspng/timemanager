using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using Newtonsoft.Json;
using TimeManager.Utilities;

namespace TimeManager.Model.Tasks
{
    /// <summary> Contains a group of task lists. </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Category : NotifyPropertyChanged
    {
        private string _name;
        private List _selectedTaskList;

        public Category(string name)
        {
            Name = name;
            TaskLists = new ObservableCollection<List>();
            Renamer = new Renamer();
            TaskListMover = new Mover<List>(TaskLists, SelectedTaskList);
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
        
        public Renamer Renamer { get; }
        public Mover<List> TaskListMover { get; set; }
        public ObservableCollection<List> TaskLists { get; private set; }
        public List SelectedTaskList
        {
            get => _selectedTaskList;
            set
            {
                _selectedTaskList = value;
                TaskListMover.SelectedElement = value;
                OnPropertyChanged(nameof(SelectedTaskList));
                SelectedTaskList.UpdateStatusBar();
            }
        }

        #region file
        
        private static readonly string FolderPath = $@"{Storage.Path}\Categories";

        private string Path => $@"{FolderPath}\{ID}.json";
        private FileIO CategoryIO => new FileIO(Path);

        public void LoadTaskLists() => TaskLists = CategoryIO.LoadData<List>();
        public void SaveTaskLists() => CategoryIO.SaveData(TaskLists);
        public void Clear() => File.Delete(Path);

        #endregion

        #region deadlines indicator

        private readonly int _maxIndicatorSize = 15;
        private readonly int _maxLeftMargin = 48;
        private readonly int _maxTopMargin = 8;

        public Thickness IndicatorMargin =>
            new Thickness(_maxLeftMargin - IndicatorSize / 2, _maxTopMargin - IndicatorSize / 2, 0, 0);

        public float IndicatorSize => _maxIndicatorSize / (float) MinimumDaysBeforeDeadline;
        private int MinimumDaysBeforeDeadline
        {
            get
            {
                int result = int.MaxValue;
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