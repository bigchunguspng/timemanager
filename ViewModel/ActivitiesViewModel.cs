﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TimeManager.Model;
using TimeManager.Model.Regular;
using TimeManager.Utilities;
using static TimeManager.ViewModel.MainWindowViewModel;

namespace TimeManager.ViewModel
{
    public class ActivitiesViewModel : NotifyPropertyChanged
    {
        private RegularActivity _selectedActivity;

        public ActivitiesViewModel()
        {
            Activities = Storage.Activities;
            ActivityMover = new Mover<RegularActivity>(Activities, SelectedActivity);
            NewDate = DateTime.Today;
        }

        public Mover<RegularActivity> ActivityMover { get; set; }
        public ObservableCollection<RegularActivity> Activities { get; set; }
        public RegularActivity SelectedActivity
        {
            get => _selectedActivity;
            set
            {
                _selectedActivity = value;
                ActivityMover.SelectedElement = value;
                OnPropertyChanged();
                UpdateSelectedActivityInfo();
                ShowInStatusBar("Alt+Q - move up | Alt+A - move down | Middle click or Double click - rename");
            }
        }

        #region selected activity info

        private readonly int _defaultPanelWidth = 200;
        public int PanelWidth => ActivitySelected ? _defaultPanelWidth : 0;

        public ObservableCollection<string> SelectedActivityDates
        {
            get
            {
                if (SelectedActivity == null)
                    return new ObservableCollection<string>();
                
                var result = new ObservableCollection<string>();
                foreach (var time in SelectedActivity.Times) 
                    result.Add(time.DateOnly());
                return result;
            }
        }
        
        public ObservableCollection<string> Analytics => new ObservableCollection<string>
        {
            "Quantity",
            $"Last week: {SelectedActivity?.HowManyTimes(new Period(7))}",
            $"Last 28 days: {SelectedActivity?.HowManyTimes(new Period(28))}",
            $"All time: {SelectedActivity?.HowManyTimes()}",
            "Average frequency (per week)",
            $"Last week: {SelectedActivity?.AverageFrequency(new Period(7))}",
            $"Last 28 days: {SelectedActivity?.AverageFrequency(new Period(28))}",
            $"All time: {SelectedActivity?.AverageFrequency()}"
        };

        public Dictionary<int, double> Intervals => SelectedActivity?.IntervalDistributionChart();
        
        private void UpdateSelectedActivityInfo()
        {
            OnPropertyChanged(nameof(PanelWidth));
            OnPropertyChanged(nameof(SelectedActivityDates));
            OnPropertyChanged(nameof(Analytics));
            OnPropertyChanged(nameof(Intervals));
        }

        #endregion

        #region commands
        
        private RelayCommand _newActivity;
        private RelayCommand _removeActivity;
        private DateTime _newDate;
        private RelayCommand _addDate;
        private RelayCommand _removeDate;


        public int IndexOfSelectedDate { get; set; }
        private bool ActivitySelected => SelectedActivity != null;

        
        public RelayCommand NewActivity => _newActivity ?? (_newActivity = new RelayCommand(o =>
                Activities.Add(new RegularActivity(true))));

        public RelayCommand RemoveActivity => _removeActivity ?? (_removeActivity = new RelayCommand(o =>
                Activities.Remove(SelectedActivity), o => ActivitySelected));

        public DateTime NewDate
        {
            get => _newDate;
            set
            {
                _newDate = value;
                OnPropertyChanged(nameof(NewDate));
            }
        }

        public RelayCommand AddDate => _addDate ?? (_addDate = new RelayCommand(o =>
        {
            SelectedActivity.AddDate(NewDate);
            UpdateSelectedActivityInfo();
        }));

        public RelayCommand RemoveDate => _removeDate ?? (_removeDate = new RelayCommand(o =>
        {
            SelectedActivity.Times.RemoveAt(IndexOfSelectedDate);
            UpdateSelectedActivityInfo();
        }));

        #endregion
    }
}