using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace KhanViewer
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class PlaylistPage : KhanViewer.Common.LayoutAwarePage
    {
        public PlaylistPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string playlistparam = e.Parameter.ToString();

            var playlist = App.ViewModel.GetPlaylist(playlistparam);
            playlist.LoadVideos();
            pageRoot.DataContext = playlist;
        }

        private void MainListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // If selected index is -1 (no selection) do nothing
            if (MainListBox.SelectedIndex == -1)
                return;
            var item = MainListBox.SelectedItem as VideoItem;
            // Navigate to the new page
            //item.Navigate();
            Frame.Navigate(typeof(VideoPlayer), item);

            // Reset selected index to -1 (no selection)
            MainListBox.SelectedIndex = -1;
        }
    }
}
