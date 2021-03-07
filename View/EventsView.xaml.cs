using System.Windows.Controls;
using System.Windows.Input;

namespace TimeManager.View
{
    public partial class EventsView : Page
    {
        public EventsView()
        {
            InitializeComponent();
        }

        private void Scroll_OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scrollViewer = (ScrollViewer) sender;
            scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - e.Delta / 5f);
            e.Handled = true;
        }
    }
}