using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using TimeManager.Model;
using TimeManager.Model.Tasks;
using TimeManager.Properties;
using TimeManager.Utilities;
using TimeManager.View;

namespace TimeManager.ViewModel
{
    public class MainWindowViewModel : NotifyPropertyChanged
    {
        private Page _categoryView;
        private Page _selectedPage;
        private Category _selectedCategory;


        public MainWindowViewModel()
        {
            Storage.LoadData();
            Categories = Storage.Categories;

            ExtraPages = new ObservableCollection<Page> {new EventsView(), new ActivitiesView()};
        }

        public static Messenger StatusBarMessenger { get; set; } = new Messenger();
        public static void ShowInStatusBar(string message) => StatusBarMessenger.Message = message;

        public ObservableCollection<Page> ExtraPages { get; }
        public Page SelectedPage
        {
            get => _selectedPage;
            set
            {
                if (value != _categoryView) SelectedCategory = null;
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
                _categoryView = new CategoryView(_selectedCategory);
                SelectedPage = _categoryView;
                OnPropertyChanged(nameof(SelectedCategory));
            }
        }

        #region window size & position

        public double Left
        {
            get => Settings.Default.Left;
            set => Settings.Default.Left = value;
        }

        public double Top
        {
            get => Settings.Default.Top;
            set => Settings.Default.Top = value;
        }

        public double Height
        {
            get => Settings.Default.Height;
            set => Settings.Default.Height = value;
        }

        public double Width
        {
            get => Settings.Default.Width;
            set => Settings.Default.Width = value;
        }

        #endregion

        #region commands

        private RelayCommand _newCategory;
        private RelayCommand _removeCategory;
        private RelayCommand _saveAll;
        private RelayCommand _restoreAll;


        public RelayCommand NewCategory => _newCategory ?? (_newCategory = new RelayCommand(o =>
            Categories.Add(new Category("New Category"))));

        public RelayCommand RemoveCategory => _removeCategory ?? (_removeCategory = new RelayCommand(o =>
            {
                Storage.RecycleBin.Add(SelectedCategory);
                int i = Storage.RecycleBin.Count;
                ShowInStatusBar($"{(i == 1 ? "One category was" : $"{i} categories were")} moved to recycle bin");
                Categories.Remove(SelectedCategory);
            }, o => CategorySelected));

        public RelayCommand SaveAll => _saveAll ?? (_saveAll = new RelayCommand(o => Storage.SaveAll()));

        public RelayCommand RestoreAll => _restoreAll ?? (_restoreAll = new RelayCommand(o =>
        {
            foreach (var category in Storage.RecycleBin)
                Categories.Add(category);
            Storage.RecycleBin.Clear();
            ShowInStatusBar("Removed categories were restored!");
        }, o => ThereAreCategoriesInRecycleBin));

        private bool CategorySelected => SelectedCategory != null;
        private bool ThereAreCategoriesInRecycleBin => Storage.RecycleBin.Count > 0;

        #endregion
    }
}