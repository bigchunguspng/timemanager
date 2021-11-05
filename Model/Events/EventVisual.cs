using System;
using System.Windows;
using TimeManager.Utilities;

namespace TimeManager.Model.Events
{
    public class EventVisual : NotifyPropertyChanged
    {
        private readonly Event _event;
        
        private double _startX;
        private double _startY;
        private double _length;
        private double _height;

        public EventVisual(Event e) => _event = e;
        
        public Thickness Start => new Thickness(_startX, _startY, 0, 0);
        public double Length => _length;
        public double Height => _height;
        
        public double L1 => (31 + 29) / YearLength(Storage.Year) * 300;
        public double L2 => (31 + 30 + 31) / YearLength(Storage.Year) * 300;
        public double L4 => (30 + 31 + 30) / YearLength(Storage.Year) * 300;
        public double L5 => 31 / YearLength(Storage.Year) * 300;
        
        public void Update(double yearLengthVisual, int year, bool adjustY = false)
        {
            double yearLength = YearLength(year);

            if (_event.Period.StartDate.Year > year)
                _startX = yearLengthVisual;
            else if (_event.Period.StartDate.Year < year)
                _startX = 0;
            else
                _startX = _event.Period.StartDate.DayOfYear / yearLength * yearLengthVisual;

            double endVisual;
            if (_event.LastDate.Year > year)
                endVisual = yearLengthVisual;
            else if (_event.LastDate.Year < year)
                endVisual = 0;
            else
                endVisual = _event.LastDate.DayOfYear / yearLength * yearLengthVisual;
            
            _length = _event.OneDay && _event.LastDate.Year == year ? 5 : endVisual - _startX;
            _height = _event.OneDay ? _event.Period.StartDate.Year == year ? 5 : 0 : 18;
            if (_event.OneDay) _startX -= 2.5;

            if (adjustY)
            {
                int yearCount = _event.LastDate.Year - _event.Period.StartDate.Year;
                int yearsAgo = Storage.Year - _event.Period.StartDate.Year;
                if (_event.OneDay)
                    _startY = (18 - 5) / 2d;
                else if (yearCount > 0) 
                    _startY = 18 * Math.Min(yearsAgo, yearCount);
            }
            else
                _startY = 0;

            UpdateView();
        }

        private double YearLength(int year) => DateTime.DaysInMonth(year, 2) + 337;

        private void UpdateView()
        {
            OnPropertyChanged(nameof(L1));
            OnPropertyChanged(nameof(L2));
            OnPropertyChanged(nameof(L4));
            OnPropertyChanged(nameof(L5));
            OnPropertyChanged(nameof(Start));
            OnPropertyChanged(nameof(Length));
            OnPropertyChanged(nameof(Height));
        }
    }
}