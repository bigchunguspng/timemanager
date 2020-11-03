using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Threading;
using TimeManager.Model.Tasks;
using TimeManager.Utilities;

namespace TimeManager.ViewModel
{
    public class MainWindowViewModel : NotifyPropertyChanged
    {
        internal static readonly string _path = @"D:\Documents\TimeManager";
        private readonly FileIO _categoriesIO;
        private Category _selectedCategory;
        private RelayCommand _newCategory;
        private RelayCommand _removeCategory;
        private RelayCommand _newList;
        private RelayCommand _removeList;
        private RelayCommand _saveAll;
        private DispatcherTimer _timer;


        public MainWindowViewModel()
        {
            _categoriesIO = new FileIO($@"{_path}\{nameof(Categories)}.json");
            Categories = new ObservableCollection<Category>();
            Directory.CreateDirectory(_path);
            LoadCategories();
        }

        public ObservableCollection<Category> Categories { get; set; }

        public Category SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                _selectedCategory = value;
                if (CategorySelected())
                    InitializeTimer();
                else
                    _timer?.Stop();
                OnPropertyChanged(nameof(SelectedCategory));
            }
        }

        #region commands

        public RelayCommand NewCategory =>
            _newCategory ?? (_newCategory = new RelayCommand(o => Categories.Add(new Category("New Category"))));

        public RelayCommand RemoveCategory =>
            _removeCategory ?? (_removeCategory = new RelayCommand(RemoveCategoryExecute,
                o => CategorySelected()));

        private void RemoveCategoryExecute(object o)
        {
            SelectedCategory.Clear();
            Categories.Remove(SelectedCategory);
        }

        public RelayCommand NewList =>
            _newList ?? (_newList = new RelayCommand(o => SelectedCategory.TaskLists.Add(new List()),
                o => CategorySelected()));

        public RelayCommand RemoveList => _removeList ?? (_removeList =
            new RelayCommand(o => SelectedCategory.TaskLists.Remove(SelectedCategory.SelectedTaskList),
                o => TaskSelected()));

        public RelayCommand SaveAll => _saveAll ?? (_saveAll = new RelayCommand(o => SaveAllExecute()));

        private void SaveAllExecute()
        {
            Directory.CreateDirectory(_path);
            _categoriesIO.SaveData(Categories);
            foreach (var category in Categories) category.SaveTaskLists();

            /*DirectoryInfo directoryInfo = new DirectoryInfo(_path + $@"\{nameof(Categories)}");
            FileInfo[] files = directoryInfo.GetFiles();
            foreach (var file in files)
            {
                //todo: delete files which don't matches any of categories name
            }*/
        }

        private bool CategorySelected() => SelectedCategory != null;
        private bool TaskSelected() => SelectedCategory?.SelectedTaskList != null;

        #endregion
        
        #region timer

        private void InitializeTimer()
        {
            if (_timer != null) return;

            _timer = new DispatcherTimer();
            _timer.Tick += TimerOnTick;
            _timer.Interval = new TimeSpan(0, 0, 1);
            _timer.Start();
        }

        private void TimerOnTick(object sender, EventArgs e)
        {
            foreach (var list in SelectedCategory.TaskLists)
            foreach (var task in list.Tasks)
                task.UpdateTimeInfo();
        }

        #endregion

        private void LoadCategories()
        {
            Categories = _categoriesIO.LoadData<Category>();
            foreach (var category in Categories) category.LoadTaskLists();
        }
    }
}