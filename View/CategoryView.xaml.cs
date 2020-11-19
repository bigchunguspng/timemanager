using System.Windows.Controls;

namespace TimeManager.View
{
    public partial class CategoryView : Page
    {
        public CategoryView(/*Category selectedCategory*/)
        {
            InitializeComponent();
            //DataContext = new CategoryViewModel(selectedCategory);
        }
    }
}