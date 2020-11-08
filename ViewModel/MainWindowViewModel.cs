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

        private Page _category;
        private Page _selectedPage;        
        private Category _selectedCategory;
        
        private RelayCommand _newCategory;
        private RelayCommand _removeCategory;
        private RelayCommand _saveAll;
        

        public MainWindowViewModel()
        {
            _categoriesIO = new FileIO($@"{_path}\{nameof(Categories)}.json");
            Categories = new ObservableCollection<Category>();
            Directory.CreateDirectory(_path);
            LoadCategories();
            
            //_category = new View.Category();
            ExtraPages = new ObservableCollection<Page> {new EventsView(), new ActivitiesView()};
        }

        public ObservableCollection<Page> ExtraPages { get; }
        public Page SelectedPage
        {
            get => _selectedPage;
            set
            {
                if (value != _category) SelectedCategory = null;
                else _selectedPage = null;
                OnPropertyChanged(nameof(SelectedPage));
                _selectedPage = value;
                OnPropertyChanged(nameof(SelectedPage));
            }
        }

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

        #endregion

        private void LoadCategories()
        {
            Categories = _categoriesIO.LoadData<Category>();
            foreach (var category in Categories) category.LoadTaskLists();
        }
    }
}