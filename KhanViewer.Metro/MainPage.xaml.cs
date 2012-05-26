using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using KhanViewer.Models;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace KhanViewer
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class MainPage : KhanViewer.Common.LayoutAwarePage
    {
        public MainPage()
        {
            this.InitializeComponent();
            UIThread.Initialize(this.Dispatcher);

            App.ViewModel.LoadData();
            DataContext = App.ViewModel;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            App.ViewModel.LoadData();
            App.ViewModel.HasUserSeenIntro(seenit =>
            {
                /* // for now, this isn't working
                 * 
                if (!seenit)
                {
                    App.ViewModel.TrackPageView("Intro", "/Intro");
                    Frame.Navigate(typeof(Intro));
                }
                else*/
                {
                    App.ViewModel.TrackPageView("Main", "/");
                }
            });


            DataContext = App.ViewModel;
        }        
        
        // Handle selection changed on ListBox
        private void MainListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // If selected index is -1 (no selection) do nothing
            if (MainListBox.SelectedIndex == -1)
                return;
            var item = MainListBox.SelectedItem as Item;
            // Navigate to the new page

            Frame.Navigate(typeof(CategoryPage), item.Name);

            MainListBox.SelectedIndex = -1;
        }
    }
}
