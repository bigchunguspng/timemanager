using System.Windows.Controls;
using TimeManager.Model.Tasks;
using TimeManager.ViewModel;

namespace TimeManager.View
{
    public partial class CategoryView : Page
    {
        public CategoryView(Category selectedCategory)
        {
            InitializeComponent();
            DataContext = new CategoryViewModel(selectedCategory);
        }
    }
}