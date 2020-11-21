using System.Windows;
using TimeManager.ViewModel;

namespace TimeManager.Utilities
{
    public class RenameControl : NotifyPropertyChanged
    {
        private RelayCommand _toggleRenameMode;

        public RenameControl()
        {
            RenameMode = Visibility.Collapsed;
        }
        
        public Visibility RenameMode { get; set; }
        public RelayCommand ToggleRenameMode => _toggleRenameMode ?? (_toggleRenameMode = new RelayCommand(o =>
        {
            if (RenameMode == Visibility.Collapsed)
            {
                RenameMode = Visibility.Visible;
                MainWindowViewModel.ShowInStatusBar("Enter or Esc - exit rename mode");
            }
            else
            {
                RenameMode = Visibility.Collapsed;
                MainWindowViewModel.ShowInStatusBar("");
            }

            OnPropertyChanged(nameof(RenameMode));
        }));
    }
}