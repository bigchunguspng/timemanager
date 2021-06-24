using System.Windows;
using static TimeManager.ViewModel.MainWindowViewModel;

namespace TimeManager.Utilities
{
    /// <summary> Allows to show text as TextBlock all time and as TextBox if it needs to be renamed. </summary>
    public class Renamer : NotifyPropertyChanged
    {
        private RelayCommand _toggleRenameMode;
        private Visibility _renameMode;

        public Renamer(bool enableRenameMode = false)
        {
            RenameMode = enableRenameMode ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary> Bind it to TextBox Visibility. </summary>
        public Visibility RenameMode
        {
            get => _renameMode;
            set
            {
                _renameMode = value;
                ActiveRenamer = value == Visibility.Visible ? this : null;
                OnPropertyChanged();
            }
        }

        public static Renamer ActiveRenamer { get; private set; }

        /// <summary> Bind it to actions that enable or disable rename mode. </summary>
        public RelayCommand ToggleRenameMode => _toggleRenameMode ?? (_toggleRenameMode = new RelayCommand(o =>
        {
            if (RenameMode == Visibility.Collapsed)
            {
                ExitOtherRenameModes();
                EnterRenameMode();
            }
            else
                ExitRenameMode();
        }));

        private void EnterRenameMode()
        {
            RenameMode = Visibility.Visible;
            ShowInStatusBar("Middle click, Enter or Esc - exit rename mode");
        }

        private void ExitRenameMode()
        {
            RenameMode = Visibility.Collapsed;
            ShowInStatusBar("");
        }

        public static void ExitOtherRenameModes() => ActiveRenamer?.ExitRenameMode();
    }
}