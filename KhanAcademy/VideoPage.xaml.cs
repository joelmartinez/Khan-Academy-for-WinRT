using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using KhanAcademy.Data;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.PlayTo;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace KhanAcademy
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class VideoPage : KhanAcademy.Common.LayoutAwarePage
    {
		private PlayToManager _playToManager = null;
		private CoreDispatcher _dispatcher = null;

        public VideoPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            VideoItem vid = navigationParameter as VideoItem;
            this.DataContext = vid;
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
        }

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			_dispatcher = Window.Current.CoreWindow.Dispatcher;
			_playToManager = PlayToManager.GetForCurrentView();
			_playToManager.SourceRequested += playToManager_SourceRequested;

			base.OnNavigatedTo(e);
		}

		protected override void OnNavigatedFrom(NavigationEventArgs e)
		{
			_playToManager.SourceRequested -= playToManager_SourceRequested;

			base.OnNavigatedFrom(e);
		}

		protected override void DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
		{
			VideoItem vi = (this.DataContext as VideoItem);
			args.Request.Data.Properties.Title = vi.Name;
			args.Request.Data.Properties.Description = vi.Description;
			args.Request.Data.SetUri(vi.KhanPath);
		}

		void playToManager_SourceRequested(PlayToManager sender, PlayToSourceRequestedEventArgs args)
		{
			var deferral = args.SourceRequest.GetDeferral();
			var handler = _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
			{
				args.SourceRequest.SetSource(videoElement.PlayToSource);
				deferral.Complete();
			});
		}
    }
}
