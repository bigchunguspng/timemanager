using System.Collections.ObjectModel;
using System.IO;
using Newtonsoft.Json;
using TimeManager.Utilities;
using TimeManager.ViewModel;

namespace TimeManager.Model.Data
{
    public class Category : NotifyPropertyChanged
    {
        private static readonly string FolderPath = $@"{MainWindowViewModel._path}\{nameof(MainWindowViewModel.Categories)}";
        private string _path;
        private string _name;
        private List _selectedTaskList;

        public Category(string name) => Name = name;

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                Directory.CreateDirectory(FolderPath);
                _path = $@"{FolderPath}\{Name}.json";
                //if (_name != null) File.Move(_path, $@"{FolderPath}\{Name}.json");
                OnPropertyChanged(nameof(Name));
                
            }
        }
        [JsonIgnore]
        public ObservableCollection<List> TaskLists { get; set; } = new ObservableCollection<List>();
        [JsonIgnore]
        public List SelectedTaskList
        {
            get => _selectedTaskList;
            set
            {
                _selectedTaskList = value;
                OnPropertyChanged(nameof(SelectedTaskList));
            }
        }

        private FileIO CategoryIO => new FileIO(_path);

        public void LoadTaskLists() => TaskLists = CategoryIO.LoadData<List>();
        public void SaveTaskLists() => CategoryIO.SaveData(TaskLists);
        public void Clear() => File.Delete(_path);
    }
}