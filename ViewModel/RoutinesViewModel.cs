using System.Collections.ObjectModel;
using TimeManager.Model.Routines;

namespace TimeManager.ViewModel
{
    public class RoutinesViewModel
    {
        public RoutinesViewModel()
        {
            Routines = new ObservableCollection<Routine>
            {
                new OnlineActivity("ахдщба"), 
                new OfflineActivity("cycle")
            };
        }
        public ObservableCollection<Routine> Routines { get; set; }
        public Routine SelectedRoutine { get; set; } //todo onchanged - change stats control (page or u\c)
    }
}