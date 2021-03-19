using System.Windows;

namespace TimeManager.View
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //without this window will overlap the taskbar
            MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight - 2;
            MaxWidth = SystemParameters.MaximizedPrimaryScreenWidth;

            Minimize.Click += (sender, args) => WindowState = WindowState.Minimized;
            Maximize.Click += (sender, args) =>
                WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
            CloseWindow.Click += (sender, args) => Close();
        }
    }
}