using System;
using System.Collections.ObjectModel;
using System.IO;
using TimeManager.Model.Regular;
using TimeManager.Utilities;

namespace TimeManager.ViewModel
{
    public class ActivitiesViewModel : NotifyPropertyChanged
    {
        private readonly FileIO _activitiesIO;
        
        private RegularActivity _selectedActivity;
        //private Page _selectedRoutineType;
        private RelayCommand _newActivity;
        private RelayCommand _removeActivity;
        private RelayCommand _addDate;
        private DateTime _newDate;
        private RelayCommand _saveActivities;

        public ActivitiesViewModel()
        {
            /*Activities = new ObservableCollection<RegularActivity>
            {
                new RegularActivity("ахдщба"), 
                new RegularActivity("cycle")
            };*/
            _activitiesIO = new FileIO($@"{MainWindowViewModel._path}\Activities.json");
            Activities = new ObservableCollection<RegularActivity>();
            Directory.CreateDirectory(MainWindowViewModel._path);
            LoadActivities();
            
            NewDate = DateTime.Today;
        }
        public ObservableCollection<RegularActivity> Activities { get; set; }

        public RegularActivity SelectedActivity
        {
            get => _selectedActivity;
            set
            {
                _selectedActivity = value;
                //UpdateSelectedRoutineType();
                OnPropertyChanged(nameof(SelectedActivity));
            }
        }

        /*public Page SelectedRoutineType
        {
            get => _selectedRoutineType;
            set
            {
                _selectedRoutineType = value;
                OnPropertyChanged(nameof(SelectedRoutineType));
            }
        }*/

        /*private void UpdateSelectedRoutineType()
        {
            if (SelectedRoutine.GetType() == typeof(OfflineActivity))
                SelectedRoutineType = new OfflineActivityControl(SelectedRoutine as OfflineActivity);
            else if (SelectedRoutine.GetType() == typeof(OnlineActivity))
                SelectedRoutineType = new OnlineActivityControl();
        }*/

        #region commands

        public RelayCommand NewActivity => 
            _newActivity ?? (_newActivity = new RelayCommand(o => Activities.Add(new RegularActivity())));

        public RelayCommand RemoveActivity =>
            _removeActivity ?? (_removeActivity = new RelayCommand(o => Activities.Remove(SelectedActivity), o => ActivitySelected()));
        
        
        public DateTime NewDate
        {
            get => _newDate;
            set
            {
                _newDate = value;
                OnPropertyChanged(nameof(NewDate));
            }
        }

        public RelayCommand AddDate =>
            _addDate ?? (_addDate = new RelayCommand(o => SelectedActivity.AddDate(NewDate)));

        private bool ActivitySelected() => SelectedActivity != null;

        public RelayCommand SaveActivities =>
            _saveActivities ?? (_saveActivities = new RelayCommand(o => SaveActivitiesExecute()));

        private void SaveActivitiesExecute()
        {
            Directory.CreateDirectory(MainWindowViewModel._path);
            _activitiesIO.SaveData(Activities);
        }

        #endregion

        private void LoadActivities()
        {
            Activities = _activitiesIO.LoadData<RegularActivity>();
        }
    }
}