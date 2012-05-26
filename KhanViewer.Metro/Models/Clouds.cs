using System.Collections.ObjectModel;

namespace KhanViewer.Models
{
    public static class Clouds
    {
        private static DataServer server;

        static Clouds()
        {
            server = new KhanAcademyApi();
        }

        /// <summary>Queries the server for a list of all categories</summary>
        public static void LoadCategoriesFromServer(ObservableCollection<GroupItem> groups, ObservableCollection<CategoryItem> items)
        {
            server.LoadCategories(groups, items);
        }

        /// <summary>Given a category, will query the server for those videos.</summary>
        public static void GetVideosFromServer(ObservableCollection<VideoItem> videolist, string categoryName)
        {
            server.LoadVideos(categoryName, videolist);
        }
    }
}
