using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace KhanViewer.Models
{
    public abstract class DataServer
    {
        public void LoadPlaylists(ObservableCollection<TopicItem> groups, ObservableCollection<PlaylistItem> items)
        {
            this.LoadPlaylists(groups, items, cats => LocalStorage.SavePlaylists(cats));
        }

        protected abstract void LoadPlaylists(ObservableCollection<TopicItem> groups, ObservableCollection<PlaylistItem> items, Action<PlaylistItem[]> localSaveAction);

        public void LoadVideos(string playlist, ObservableCollection<VideoItem> items)
        {
            this.LoadVideos(playlist, items, vids => LocalStorage.SaveVideos(playlist, vids));
        }

        protected abstract void LoadVideos(string playlist, ObservableCollection<VideoItem> items, Action<VideoItem[]> localSaveAction);
    }
}
