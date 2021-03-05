using System.Windows;

namespace TimeManager.View
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Minimize.Click += (sender, args) => WindowState = WindowState.Minimized;
            Maximize.Click += (sender, args) =>
                WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
            CloseWindow.Click += (sender, args) => Close();
        }
    }
}