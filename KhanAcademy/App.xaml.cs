using KhanAcademy.Common;
using KhanAcademy.Data;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Search;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Grid App template is documented at http://go.microsoft.com/fwlink/?LinkId=234226

namespace KhanAcademy
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
		public static KhanDataSource DataSource { get; private set; }

        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
            this.UnhandledException += App_UnhandledException;
        }

        void App_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            e.Handled = true;

            MessageDialog dialog = new MessageDialog(e.Exception.Message, "Sorry!");
            dialog.ShowAsync();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
			LaunchApp(args.PreviousExecutionState);
		}

		public async void LaunchApp(ApplicationExecutionState previousExecutionState)
		{
            DataSource = new KhanDataSource();
            await DataSource.LoadAllData();

			SearchPane searchPane = SearchPane.GetForCurrentView();
			searchPane.PlaceholderText = "Search Khan Academy";
			searchPane.QuerySubmitted +=
				(sender, queryArgs) =>
				{
					KhanAcademy.SearchResultsPage.Activate(queryArgs.QueryText, previousExecutionState);
				};

			searchPane.SuggestionsRequested += 
				(sender, suggestionArgs) =>
				{
					var videos = App.DataSource.TopicGroups.SelectMany(g =>
						g.Playlists.SelectMany(p => p.Videos.Where(v =>
							Regex.IsMatch(v.Name ?? "", suggestionArgs.QueryText, RegexOptions.IgnoreCase) ||
							Regex.IsMatch(v.Description ?? "", suggestionArgs.QueryText, RegexOptions.IgnoreCase))))
                            .Distinct(VideoItem.CreateComparer())
                            .Take(3);

					foreach(VideoItem vi in videos)
						suggestionArgs.Request.SearchSuggestionCollection.AppendQuerySuggestion(vi.Title);

					var recommended = App.DataSource.TopicGroups.SelectMany(g =>
						g.Playlists.SelectMany(p => p.Videos.Where(v =>
							Regex.IsMatch(v.Name ?? "", suggestionArgs.QueryText, RegexOptions.IgnoreCase)))).FirstOrDefault();

					if(recommended != null)
					{
						suggestionArgs.Request.SearchSuggestionCollection.AppendSearchSeparator("Recommended");

						IRandomAccessStreamReference imgStream = RandomAccessStreamReference.CreateFromUri(recommended.ImagePath);
						suggestionArgs.Request.SearchSuggestionCollection.AppendResultSuggestion(recommended.Title, recommended.Description, recommended.VideoPath.ToString(), imgStream, recommended.Title);
					}
				};

			searchPane.ResultSuggestionChosen +=
				(sender, resultArgs) =>
				{
					var recommended = App.DataSource.TopicGroups.SelectMany(g =>
						g.Playlists.SelectMany(p => p.Videos.Where(v =>
							Regex.IsMatch(v.VideoPath.ToString() ?? "", resultArgs.Tag, RegexOptions.IgnoreCase)))).FirstOrDefault();

					Frame f = Window.Current.Content as Frame;
					f.Navigate(typeof(VideoPage), JsonSerializer.Serialize(recommended));
				};

            if (previousExecutionState == ApplicationExecutionState.Running)
            {
                Window.Current.Activate();
                return;
            }

            var rootFrame = new Frame();
            SuspensionManager.RegisterFrame(rootFrame, "AppFrame");

            if (previousExecutionState == ApplicationExecutionState.Terminated)
            {
                await SuspensionManager.RestoreAsync();
            }

            if (rootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                if (!rootFrame.Navigate(typeof(HubPage), JsonSerializer.Serialize(DataSource.TopicGroups)))
                {
                    throw new Exception("Failed to create initial page");
                }
            }

            // Place the frame in the current Window and ensure that it is active
            Window.Current.Content = rootFrame;
            Window.Current.Activate();
        }

        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            await SuspensionManager.SaveAsync();  // THIS CAUSED ERROR WHEN REMOTE MACHINE ( Slate ) TESTING
            deferral.Complete();
        }

        /// <summary>
        /// Invoked when the application is activated to display search results.
        /// </summary>
        /// <param name="args">Details about the activation request.</param>
		protected override async void OnSearchActivated(Windows.ApplicationModel.Activation.SearchActivatedEventArgs args)
		{
			DataSource = new KhanDataSource();
			await DataSource.LoadAllData();
			KhanAcademy.SearchResultsPage.Activate(args.QueryText, args.PreviousExecutionState);
		}
    }
}
