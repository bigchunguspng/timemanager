using System.Windows;
using static TimeManager.ViewModel.MainWindowViewModel;

namespace TimeManager.Utilities
{
    /// <summary> Allows to show text as TextBlock all time and as TextBox if it needs to be renamed. </summary>
    public class Renamer : NotifyPropertyChanged
    {
        private RelayCommand _toggleRenameMode;

        public Renamer(bool enableRenameMode = false)
        {
            RenameMode = enableRenameMode ? Visibility.Visible : Visibility.Collapsed;
        }
        
        /// <summary> Bind it to TextBox Visibility. </summary>
        public Visibility RenameMode { get; set; }
        
        public static Renamer ActiveRenamer { get; private set; }

        /// <summary> Bind it to actions that enable or disable rename mode. </summary>
        public RelayCommand ToggleRenameMode => _toggleRenameMode ?? (_toggleRenameMode = new RelayCommand(o =>
        {
            if (RenameMode == Visibility.Collapsed) //enter rename mode
            {
                ExitOtherRenameModes();
                RenameMode = Visibility.Visible;
                ActiveRenamer = this;
                ShowInStatusBar("Middle click, Enter or Esc - exit rename mode");
                
                OnPropertyChanged(nameof(RenameMode));
            }
            else
                ExitRenameMode();
        }));

        private void ExitRenameMode()
        {
            RenameMode = Visibility.Collapsed;
            ActiveRenamer = null;
            ShowInStatusBar("");
            
            OnPropertyChanged(nameof(RenameMode));
        }
        
        public static void ExitOtherRenameModes() => ActiveRenamer?.ExitRenameMode();
    }
}