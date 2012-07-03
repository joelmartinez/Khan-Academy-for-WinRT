using KhanAcademy.Data;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.ApplicationModel.DataTransfer;
using System.Collections.ObjectModel;

// The Grouped Items Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234231

namespace KhanAcademy
{
    /// <summary>
    /// A page that displays a grouped collection of items.
    /// </summary>
    public sealed partial class HubPage : KhanAcademy.Common.LayoutAwarePage
    {

        public HubPage()
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
			ObservableCollection<TopicItem> items = JsonSerializer.Deserialize<ObservableCollection<TopicItem>>(navigationParameter as string);
			this.DefaultViewModel["Groups"] = items;
			this.groupGridView.ItemsSource = items;
        }

        void Header_Click(object sender, RoutedEventArgs e)
        {
            
            var group = (sender as FrameworkElement).DataContext;
            TopicItem topicSelected = (group as TopicItem);

            if (topicSelected != null)
            {
                JumpToTopic(topicSelected);
            }
        }

        void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem.GetType() == typeof(PlaylistItem))
            {
                this.Frame.Navigate(typeof(ItemDetailPage), JsonSerializer.Serialize(e.ClickedItem));
            }
            else if (e.ClickedItem.GetType() == typeof(VideoItem))
            {
                // CALL UP THE PLAY VIDEO PAGE
                this.Frame.Navigate(typeof(VideoPage), JsonSerializer.Serialize(e.ClickedItem));
            }
        }

        private void groupTitle_Click(object sender, ItemClickEventArgs e)
        {
            TopicItem topicSelected = (e.ClickedItem as TopicItem);

            if (topicSelected != null)
                JumpToTopic(topicSelected);
        }

        void JumpToTopic(TopicItem topicSelected)
        {
            if (topicSelected.ContentType == TopicContentType.Videolist)
            {
                this.Frame.Navigate(typeof(ItemDetailPage), JsonSerializer.Serialize(topicSelected.Playlists[0]));
            }
            else
            {
                this.Frame.Navigate(typeof(GroupDetailPage), JsonSerializer.Serialize(topicSelected));
            }
        }

        // This appears to be a work around required for Semantic Zoom Out swallowing the
        // ItemClicked events on a GridView on Samsung Tablet via Touch. 
        // Bug reported to MSFT by Jamie Rodriguez but still unresolved as of 6/20/2012.
        private void groupTitle_Pressed(object sender, PointerRoutedEventArgs e)
        {
            var group = (e.OriginalSource as FrameworkElement).DataContext;
            TopicItem topicSelected = (group as TopicItem);

            if (topicSelected != null)
            {
                JumpToTopic(topicSelected);
	         }
	    }
        
    }
}
