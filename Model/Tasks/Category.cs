using System.Collections.ObjectModel;
using System.IO;
using Newtonsoft.Json;
using TimeManager.Utilities;
using TimeManager.ViewModel;

namespace TimeManager.Model.Tasks
{
    public class Category : NotifyPropertyChanged
    {
        private static readonly string FolderPath = $@"{MainWindowViewModel._path}\{nameof(MainWindowViewModel.Categories)}";
        private string _name;
        private List _selectedTaskList;

        public Category(string name)
        {
            Name = name;
            TaskLists = new ObservableCollection<List>();
            ID = Hash.UniqueHash(FolderPath);
        }

        public string Name
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

        private string Path => $@"{FolderPath}\{ID}.json";
        private FileIO CategoryIO => new FileIO(Path);

        public void LoadTaskLists() => TaskLists = CategoryIO.LoadData<List>();
        public void SaveTaskLists() => CategoryIO.SaveData(TaskLists);
        public void Clear() => File.Delete(Path);
    }
}