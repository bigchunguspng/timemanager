using System.Collections.ObjectModel;

namespace TimeManager.Utilities
{
    public class Mover<T>
    {
        private RelayCommand _moveUp;
        private RelayCommand _moveDown;
        
        public Mover(ObservableCollection<T> collection, T selectedElement)
        {
            Collection = collection;
            SelectedElement = selectedElement;
        }

        public ObservableCollection<T> Collection { get; set; }
        public T SelectedElement { get; set; }

        public RelayCommand MoveUp => _moveUp ?? (_moveUp = new RelayCommand(o =>
        {
            int index = SelectedElementIndex;
            Collection.Move(index, index - 1);
        }, o => ElementSelected && ElementIsNotFirst));

        public RelayCommand MoveDown => _moveDown ?? (_moveDown = new RelayCommand(o =>
        {
            int index = SelectedElementIndex;
            Collection.Move(index, index + 1);
        }, o => ElementSelected && ElementIsNotLast));

        private bool ElementSelected => SelectedElement != null;
        private bool ElementIsNotFirst => SelectedElementIndex > 0;
        private bool ElementIsNotLast => SelectedElementIndex < Collection.Count - 1;
        private int SelectedElementIndex => Collection.IndexOf(SelectedElement);

        
    }
}