﻿using System;
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
        private RelayCommand _newActivity;
        private RelayCommand _removeActivity;
        private RelayCommand _addDate;
        private DateTime _newDate;
        private RelayCommand _saveActivities;
        private RelayCommand _removeDate;

        public ActivitiesViewModel()
        {
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
                OnPropertyChanged(nameof(SelectedActivity));
                UpdateSelectedActivityInfo();
            }
        }

        #region selected activity info

        private readonly int _defaultpanelwidth = 200;
        public int PanelWidth => ActivitySelected ? _defaultpanelwidth : 0;

        public ObservableCollection<string> SelectedActivityDates
        {
            get
            {
                var result = new ObservableCollection<string>();
                foreach (var time in SelectedActivity.Times) 
                    result.Add(DateExtensions.DateOnly(time));
                return result;
            }
        }
        
        public ObservableCollection<string> Analytics => new ObservableCollection<string>
        {
            "Quantity",
            $"Last week: {SelectedActivity.HowManyTimes(new Period(7))}",
            $"Last 28 days: {SelectedActivity.HowManyTimes(new Period(28))}",
            $"All time: {SelectedActivity.HowManyTimes(new Period(SelectedActivity.Times[0], DateTime.Today))}",
            "Average frequency (per week)",
            $"Last week: {SelectedActivity.AverageFrequency(new Period(7))}",
            $"Last 28 days: {SelectedActivity.AverageFrequency(new Period(28))}",
            $"All time: {SelectedActivity.AverageFrequency(new Period(SelectedActivity.Times[0], DateTime.Today))}"
        };

        private void UpdateSelectedActivityInfo()
        {
            OnPropertyChanged(nameof(PanelWidth));
            OnPropertyChanged(nameof(SelectedActivityDates));
            OnPropertyChanged(nameof(Analytics));
        }

        #endregion

        #region commands

        public RelayCommand NewActivity => 
            _newActivity ?? (_newActivity = new RelayCommand(o => Activities.Add(new RegularActivity())));

        public RelayCommand RemoveActivity =>
            _removeActivity ?? (_removeActivity = new RelayCommand(o => Activities.Remove(SelectedActivity), o => ActivitySelected));
        
        
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
            _addDate ?? (_addDate = new RelayCommand(AddDateExecute));

        private void AddDateExecute(object o)
        {
            SelectedActivity.AddDate(NewDate);
            UpdateSelectedActivityInfo();
        }

        public int IndexOfSelectedDate { get; set; }

        public RelayCommand RemoveDate =>
            _removeDate ?? (_removeDate = new RelayCommand(RemoveDateExecute));

        private void RemoveDateExecute(object o)
        {
            SelectedActivity.Times.RemoveAt(IndexOfSelectedDate);
            UpdateSelectedActivityInfo();
        }


        public bool ActivitySelected => SelectedActivity != null;

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