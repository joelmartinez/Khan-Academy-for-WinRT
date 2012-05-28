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

        /// <summary>Queries the server for a list of all playlists</summary>
        public static void LoadPlaylistsFromServer(ObservableCollection<GroupItem> groups, ObservableCollection<PlaylistItem> items)
        {
            server.LoadPlaylists(groups, items);
        }

        /// <summary>Given a playlist, will query the server for those videos.</summary>
        public static void GetVideosFromServer(ObservableCollection<VideoItem> videolist, string playlistName)
        {
            server.LoadVideos(playlistName, videolist);
        }
    }
}
