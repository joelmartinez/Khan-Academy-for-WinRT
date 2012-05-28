using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using KhanViewer.Models;

#if WINDOWS_PHONE
using Microsoft.Phone.Shell;
#endif

namespace KhanViewer
{
    public class MainViewModel : INotifyPropertyChanged
    {

        public MainViewModel()
        {
            this.Groups = new ObservableCollection<GroupItem>();
            this.Playlists = new ObservableCollection<PlaylistItem>();
        }

        #region Properties

        public ObservableCollection<GroupItem> Groups { get; private set; }

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
            UIThread.MessageBox(message);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        public PlaylistItem GetPlaylist(string playlistName)
        {
            var playlist = this.Playlists.Where(c => c.Name == playlistName).FirstOrDefault();
            playlist.LoadVideos();

            return playlist;
        }

        public void GetVideo(string playlistName, string name, Action<VideoItem> result)
        {
            LocalStorage.GetVideo(playlistName, name, vid =>
                {
                    if (vid != null)
                    {
                        result(vid);
                        return;
                    }

                    // didn't have the vid on disk, query the memory store.
                    vid = this.Playlists
                        .Where(c => c.Name == playlistName)
                        .SelectMany(c => c.Videos)
                        .SingleOrDefault(v => v.Name == name);

                    if (vid != null) LocalStorage.SaveVideo(vid);

                    result(vid);
                });
        }

        /// <summary>
        /// Creates and adds a few ItemViewModel objects into the Items collection.
        /// </summary>
        public void LoadData()
        {
            if (!this.IsDataLoaded)
            {
                this.IsDataLoaded = true;
                
                PlaylistItem.Initialize(this.Groups, this.Playlists);
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