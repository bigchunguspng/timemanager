using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TimeManager.Annotations;

namespace TimeManager.Model.Data
{
    public class Category : INotifyPropertyChanged
    {
        private string _name;

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
        public List<List> TaskLists { get; set; } = new List<List>();

        public void TestTaskLists() //todo ability to create lists from UI
        {
            TaskLists.Add(new List{Name = "Project #1"});
            TaskLists.Add(new List{Name = "Project #2"});
            TaskLists.Add(new List{Name = "Project #??"});
        }

        #region stuff
        public override string ToString() => Name;

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
        
    }
}