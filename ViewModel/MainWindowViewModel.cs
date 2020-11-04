using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Controls;
using System.Windows.Threading;
using TimeManager.Model.Tasks;
using TimeManager.Utilities;
using TimeManager.View;
using Category = TimeManager.Model.Tasks.Category;

namespace TimeManager.ViewModel
{
    public class MainWindowViewModel : NotifyPropertyChanged
    {
        internal static readonly string _path = @"D:\Documents\TimeManager";
        private readonly FileIO _categoriesIO;
        private Category _selectedCategory;
        private RelayCommand _newCategory;
        private RelayCommand _removeCategory;
        //private RelayCommand _newList;
        //private RelayCommand _removeList;
        private RelayCommand _saveAll;
        //private DispatcherTimer _timer;
        private RelayCommand _viewEvents;

        private Page _category;
        private readonly Page _events;
        private readonly Page _routines;
        private RelayCommand _viewRoutines;
        private Page _selectedPage;


        public MainWindowViewModel()
        {
            _categoriesIO = new FileIO($@"{_path}\{nameof(Categories)}.json");
            Categories = new ObservableCollection<Category>();
            Directory.CreateDirectory(_path);
            LoadCategories();
            
            //_category = new View.Category();
            _events = new EventsView();
            _routines = new RoutinesView();
            
        }

        public Page SelectedPage
        {
            get => _selectedPage;
            set
            {
                _selectedPage = value;
                OnPropertyChanged(nameof(SelectedPage));
            }
        }

        public RelayCommand ViewEvents => 
            _viewEvents ?? (_viewEvents = new RelayCommand(o => SelectedPage = _events));
        public RelayCommand ViewRoutines =>
            _viewRoutines ?? (_viewRoutines = new RelayCommand(o => SelectedPage = _routines));

        public ObservableCollection<Category> Categories { get; set; }

        public Category SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                _selectedCategory = value;
                /*if (CategorySelected())
                {
                    InitializeTimer();
                    _category = new CategoryView(_selectedCategory);
                    SelectedPage = _category;
                }
                else
                    _timer?.Stop();*/
                /*if (CategorySelected())
                {
                    _category = new CategoryView(_selectedCategory);
                    SelectedPage = _category;
                }*/
                _category = new CategoryView(_selectedCategory);
                SelectedPage = _category;
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

        /*public RelayCommand NewList =>
            _newList ?? (_newList = new RelayCommand(o => SelectedCategory.TaskLists.Add(new List()),
                o => CategorySelected()));

        public RelayCommand RemoveList => _removeList ?? (_removeList =
            new RelayCommand(o => SelectedCategory.TaskLists.Remove(SelectedCategory.SelectedTaskList),
                o => TaskSelected()));*/

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
        /*private bool TaskSelected() => SelectedCategory?.SelectedTaskList != null;*/

        #endregion
        
        /*#region timer

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

        #endregion*/

        private void LoadCategories()
        {
            Categories = _categoriesIO.LoadData<Category>();
            foreach (var category in Categories) category.LoadTaskLists();
        }
    }
}