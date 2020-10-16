﻿using System.Collections.ObjectModel;
using System.IO;
using TimeManager.Model.Data;
using TimeManager.Utilities;

namespace TimeManager.ViewModel
{
    public class MainWindowViewModel : NotifyPropertyChanged
    {
        internal static readonly string _path = @"D:\Documents\TimeManager";
        private readonly FileIO _categoriesIO = new FileIO($@"{_path}\{nameof(Categories)}.json");
        private ObservableCollection<Category> _categories = new ObservableCollection<Category>();
        private Category _selectedCategory;
        private RelayCommand _newCategory;
        private RelayCommand _removeCategory;
        private RelayCommand _newList;
        private RelayCommand _removeList;
        private RelayCommand _saveAll;


        public MainWindowViewModel()
        {
            Directory.CreateDirectory(_path);
            LoadCategories();
        }

        public ObservableCollection<Category> Categories
        {
            get => _categories;
            set
            {
                _categories = value;
                //_categotiesIO.SaveData(Categories);
            }
        }

        public Category SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                _selectedCategory = value;
                OnPropertyChanged(nameof(SelectedCategory));
            }
        }

        #region commands

        public RelayCommand NewCategory =>
            _newCategory ?? (_newCategory = new RelayCommand(o => Categories.Add(new Category("New Category"))));

        public RelayCommand RemoveCategory =>
            _removeCategory ?? (_removeCategory = new RelayCommand(RemoveCategoryExecute,
                o => CategoryNotSelected()));
        private void RemoveCategoryExecute(object o)
        {
            SelectedCategory.Clear();
            Categories.Remove(SelectedCategory);
        }

        public RelayCommand NewList =>
            _newList ?? (_newList = new RelayCommand(o => SelectedCategory.TaskLists.Add(new List()),
                o => CategoryNotSelected()));

        public RelayCommand RemoveList => _removeList ?? (_removeList =
            new RelayCommand(o => SelectedCategory.TaskLists.Remove(SelectedCategory.SelectedTaskList),
                o => CategoryNotSelected() && TaskNotSelected()));

        public RelayCommand SaveAll => _saveAll ?? (_saveAll = new RelayCommand(o => SaveAllExecute()));
        private void SaveAllExecute()
        {
            _categoriesIO.SaveData(Categories);
            foreach (var category in Categories) category.SaveTaskLists();
            
            /*DirectoryInfo directoryInfo = new DirectoryInfo(_path + $@"\{nameof(Categories)}");
            FileInfo[] files = directoryInfo.GetFiles();
            foreach (var file in files)
            {
                //todo: delete files which don't matches any of categories name
            }*/
        }
        
        private bool CategoryNotSelected() => SelectedCategory != null;
        private bool TaskNotSelected() => SelectedCategory?.SelectedTaskList != null;

        #endregion

        private void LoadCategories()
        {
            Categories = _categoriesIO.LoadData<Category>();
            foreach (var category in Categories) category.LoadTaskLists();
        }
    }
}