using System.Collections.ObjectModel;
using System.Windows.Controls;
using TimeManager.Model;
using TimeManager.Utilities;
using TimeManager.View;
using Category = TimeManager.Model.Tasks.Category;

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
            get => Properties.Settings.Default.Left;
            set => Properties.Settings.Default.Left = value;
        }

        public double Top
        {
            get => Properties.Settings.Default.Top;
            set => Properties.Settings.Default.Top = value;
        }

        public double Height
        {
            get => Properties.Settings.Default.Height;
            set => Properties.Settings.Default.Height = value;
        }

        public double Width
        {
            get => Properties.Settings.Default.Width;
            set => Properties.Settings.Default.Width = value;
        }

        #endregion
        
        #region commands
        
        private RelayCommand _newCategory;
        private RelayCommand _removeCategory;
        private RelayCommand _saveAll;
        

        public RelayCommand NewCategory => _newCategory ?? (_newCategory = new RelayCommand(o => 
                Categories.Add(new Category("New Category"))));

        public RelayCommand RemoveCategory => _removeCategory ?? (_removeCategory = new RelayCommand(o =>
            {
                SelectedCategory.Clear();
                Categories.Remove(SelectedCategory);
            },
            o => CategorySelected()));

        public RelayCommand SaveAll => _saveAll ?? (_saveAll = new RelayCommand(o => Storage.SaveAll()));
        
        private bool CategorySelected() => SelectedCategory != null;

        #endregion
    }
}