using System.Windows;
using Newtonsoft.Json;

namespace TimeManager.Utilities
{
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class Collapsible : NotifyPropertyChanged
    {
        private RelayCommand _toggleContentVisibility;
        
        protected delegate void Handler();

        protected event Handler VisibilityChanged;
        
        [JsonProperty] public Visibility ContentVisibility { get; set; }
        
        public RelayCommand ToggleContentVisibility =>
            _toggleContentVisibility ?? (_toggleContentVisibility = new RelayCommand(o =>
            {
                ContentVisibility = ContentVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
                OnPropertyChanged(nameof(ContentVisibility));
                VisibilityChanged?.Invoke();
            }));
    }
}