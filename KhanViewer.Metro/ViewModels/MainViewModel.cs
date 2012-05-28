using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using KhanViewer.Models;

namespace KhanViewer
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public MainViewModel()
        {
            this.Topics = new ObservableCollection<TopicItem>();
            this.Playlists = new ObservableCollection<PlaylistItem>();
        }

        #region Properties

        public ObservableCollection<TopicItem> Topics { get; private set; }

        public ObservableCollection<PlaylistItem> Playlists { get; private set; }

        public bool IsDataLoaded
        {
            get;
            private set;
        }

        /// <summary>If <see cref="IsError" /> is true, then this will contain the fault error.
        /// You should not show this to the user, but communicate to devs.</summary>
        public string ErrorMessage { get; private set; }

        /// <summary>If the application encounters an error condition, call this method.</summary>
        /// <param name="message">The error details to send to the developers.</param>
        public void SetError(string message)
        {
            UIHelper.MessageBox(message);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        public PlaylistItem GetPlaylist(string playlistName)
        {
            var playlist = this.Playlists.Where(c => c.Name == playlistName).FirstOrDefault();
            playlist.LoadVideos();

            return playlist;
        }

        public async Task<VideoItem> GetVideo(string playlistName, string name)
        {
            var vid = await LocalStorage.GetVideo(playlistName, name);
            if (vid != null) return vid;

            // didn't have the vid on disk, query the memory store.
            vid = this.Playlists
                .Where(c => c.Name == playlistName)
                .SelectMany(c => c.Videos)
                .SingleOrDefault(v => v.Name == name);

            if (vid != null) LocalStorage.SaveVideo(vid);

            return vid;
        }

        /// <summary>
        /// Creates and adds a few ItemViewModel objects into the Items collection.
        /// </summary>
        public void LoadData()
        {
            if (!this.IsDataLoaded)
            {
                this.IsDataLoaded = true;

                PlaylistItem.Initialize(this.Topics, this.Playlists);
            }
        }

        #region Private Methods

        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}