using System;
using System.Windows.Threading;
using TimeManager.Model;
using TimeManager.Model.Tasks;
using TimeManager.Utilities;

namespace TimeManager.ViewModel
{
    public class CategoryViewModel : NotifyPropertyChanged
    {
        private Category _selectedCategory;

        public CategoryViewModel()
        {
            SelectedCategory = Storage.SelectedCategory;
        }

        public Category SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                _selectedCategory = value;
                SelectedCategory.TaskListMover.Collection = value.TaskLists;
                if (CategorySelected)
                    InitializeTimer();
                else
                    _timer?.Stop();
                OnPropertyChanged(nameof(SelectedCategory));
            }
        }

        #region commands

        private RelayCommand _newList;
        private RelayCommand _removeList;


        public RelayCommand NewList => _newList ?? (_newList = new RelayCommand(
            o => SelectedCategory.TaskLists.Add(new List("New List"))));

        public RelayCommand RemoveList => _removeList ?? (_removeList = new RelayCommand(
            o => SelectedCategory.TaskLists.Remove(SelectedCategory.SelectedTaskList),
            o => TaskListSelected));
        

        private bool CategorySelected => SelectedCategory != null;
        private bool TaskListSelected => SelectedCategory?.SelectedTaskList != null;

        #endregion
        
        #region timer
        
        private DispatcherTimer _timer;

        private void InitializeTimer()
        {
            if (_timer != null) return;

            _timer = new DispatcherTimer();
            _timer.Tick += TimerOnTick;
            _timer.Interval = new TimeSpan(0, 0, 1);
            _timer.Start();
        }

        private void TimerOnTick(object sender, EventArgs e)
        {
            foreach (var list in SelectedCategory.TaskLists)
            foreach (var task in list.Tasks)
                task.UpdateTimeInfo();
        }

        #endregion

    }
}