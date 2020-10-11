using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TimeManager.Annotations;
using TimeManager.Model.Data;

namespace TimeManager.ViewModel
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private Category _selectedCategory;
        public List<Category> Categories { get; set; } = new List<Category>();

        public Category SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                _selectedCategory = value;
                OnPropertyChanged(nameof(SelectedCategory));
            }
        }

        public void TestCategoriesList() //todo ability to create a new category from UI
        {
            Categories.Add(new Category {Name = "Undefined"});
            Category c1 = new Category {Name = "Untitled"};
            c1.TestTaskLists();
            Categories.Add(c1);
            Category c2 = new Category {Name = "A28"};
            c2.TaskLists.Add(new List{Name = "Add to docs"});
            c2.TaskLists.Add(new List{Name = "Versions"});
            c2.TaskLists.Add(new List{Name = "To show"});
            Categories.Add(c2);
        }

        public MainWindowViewModel()
        {
            TestCategoriesList();
        }


        #region stuff

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}