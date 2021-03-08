using System.Collections.ObjectModel;
using System.Windows.Controls;
using TimeManager.Model;
using TimeManager.Model.Tasks;
using TimeManager.Utilities;
using TimeManager.View;
using static TimeManager.Properties.Settings;

namespace TimeManager.ViewModel
{
    public class MainWindowViewModel : NotifyPropertyChanged
    {
        private Page _selectedPage;
        private Page _selectedSection;
        private Category _selectedCategory;


        public MainWindowViewModel()
        {
            Storage.LoadData();
            Categories = Storage.Categories;
            CategoryMover = new Mover<Category>(Categories, SelectedCategory);

            DefaultSections = new ObservableCollection<Page> {new EventsView(), new ActivitiesView()};
        }

        public Page SelectedPage
        {
            get => _selectedPage;
            set
            {
                _selectedPage = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Page> DefaultSections { get; }        // "Events" & "Regular activities"
        public Page SelectedSection
        {
            get => _selectedSection;
            set
            {
                _selectedSection = value;
                OnPropertyChanged();
                if (_selectedSection != null)
                {
                    SelectedCategory = null;
                    SelectedPage = _selectedSection;
                    ShowInStatusBar("");
                }
            }
        }

        public Mover<Category> CategoryMover { get; set; }
        public ObservableCollection<Category> Categories { get; set; }    // custom categories
        public Category SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                _selectedCategory = value;
                CategoryMover.SelectedElement = value;
                Storage.SelectedCategory = SelectedCategory;
                OnPropertyChanged();
                if (CategorySelected)
                {
                    SelectedSection = null;
                    SelectedPage = new CategoryView();
                    ShowInStatusBar("Alt+Q - move up | Alt+A - move down | Middle click or Double click - rename");
                }
            }
        }

        #region status bar

        public static Messenger StatusBarMessenger { get; set; } = new Messenger();
        public static void ShowInStatusBar(string message) => StatusBarMessenger.Message = message;

        #endregion

        #region window size & position

        public double Left
        {
            get => Default.Left;
            set => Default.Left = value;
        }

        public double Top
        {
            get => Default.Top;
            set => Default.Top = value;
        }

        public double Height
        {
            get => Default.Height;
            set => Default.Height = value;
        }

        public double Width
        {
            get => Default.Width;
            set => Default.Width = value;
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
                int count = Storage.RecycleBin.Count;
                ShowInStatusBar($"{(count == 1 ? "One category was" : $"{count} categories were")} moved to recycle bin");
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