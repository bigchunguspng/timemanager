﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TimeManager.Model.Data;
using TimeManager.Properties;
using TimeManager.Utilities;
using TimeManager.View;

namespace TimeManager.ViewModel
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private Category _selectedCategory;
        private RelayCommand _newCategory;
        private RelayCommand _removeCategory;

        public MainWindowViewModel()
        {
            LoadCategoriesReplacement();
        }

        #region stuff

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        public ObservableCollection<Category> Categories { get; set; } = new ObservableCollection<Category>();
        public Category SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                _selectedCategory = value;
                OnPropertyChanged(nameof(SelectedCategory));
            }
        }
        
        public RelayCommand NewCategory =>
            _newCategory ?? (_newCategory = new RelayCommand(o => Categories.Add(new Category("New Category"))));

        public RelayCommand RemoveCategory =>
            _removeCategory ?? (_removeCategory = new RelayCommand(o => Categories.Remove(SelectedCategory)));

        private void LoadCategoriesReplacement() //todo: replace with LoadCategories() from json
        {
            Categories.Add(new Category("Undefined"));
            var c1 = new Category("Untitled");
            c1.TestTaskLists();
            Categories.Add(c1);
            var c2 = new Category("A28");
            c2.TaskLists.Add(new List {Name = "Add to docs"});
            c2.TaskLists.Add(new List {Name = "Versions"});
            c2.TaskLists.Add(new List {Name = "To show"});
            Categories.Add(c2);
        }
    }
}