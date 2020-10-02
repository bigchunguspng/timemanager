using System.Windows;
using TimeManager.Data;
using static TimeManager.Data.General;

namespace TimeManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Category c1 = new Category {Name = "RYTP"};
            Category c2 = new Category {Name = "SDTFMTD"};
            if (Categories != null)
            {
                Categories.Add(c1);
                Categories.Add(c2);

                SideMenu.ItemsSource = Categories.ToArray();
            }

            //SideMenu.ItemsSource = new[] {new Category {Name = "c1"}, new Category {Name = "c2"}};
        }
    }
}