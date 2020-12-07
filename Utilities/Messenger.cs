namespace TimeManager.Utilities
{
    public class Messenger : NotifyPropertyChanged
    {
        private string _message;

        public string Message
        {
            get => _message;
            set
            {
                _message = value;
                OnPropertyChanged();
            }
        }
    }
}