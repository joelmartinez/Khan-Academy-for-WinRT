using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using GoogleAnalyticsTracker;
using KhanViewer.Models;

#if WINDOWS_PHONE
using Microsoft.Phone.Shell;
#endif

namespace KhanViewer
{
    public class MainViewModel : INotifyPropertyChanged
    {
#if WINDOWS_PHONE
        Tracker tracker = new Tracker("UA-859807-2", "http://khanacademyforwindowsphone.com");
#else
        Tracker tracker = new Tracker("UA-859807-3", "http://khanacademyforwindowsrt.com");
#endif

        public MainViewModel()
        {
            this.Groups = new ObservableCollection<GroupItem>();
            this.Categories = new ObservableCollection<CategoryItem>();
        }

        #region Properties

        public ObservableCollection<GroupItem> Groups { get; private set; }

        public ObservableCollection<CategoryItem> Categories { get; private set; }

        public bool Querying { get; set; }

        public bool IsDataLoaded
        {
            get;
            private set;
        }

        /// <summary>If <see cref="IsError" /> is true, then this will contain the fault error.
        /// You should not show this to the user, but communicate to devs.</summary>
        public string ErrorMessage { get; private set; }

        /// <summary>If this is true, there has been a fault and you should let the user know.</summary>
        public bool IsError { get; private set; }

        /// <summary>If the application encounters an error condition, call this method.</summary>
        /// <param name="message">The error details to send to the developers.</param>
        public void SetError(string message)
        {
            UIThread.MessageBox(message);
            this.IsError = true;
            this.ErrorMessage = message;

            this.NotifyPropertyChanged("ErrorMessage");
            this.NotifyPropertyChanged("IsError");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        /// <summary>call this any time you begin a server query</summary>
        /// <returns>a handle which should wrap the operation in a using statement.</returns>
        public IDisposable StartQuerying()
        {
            // TODO: implement refcounting
            Querying = true;
            UIThread.Invoke(() => NotifyPropertyChanged("Querying"));
            return new QueryingHandle(this);
        }

        /// <returns>true only the first time the user accesses the application.</returns>
        public void HasUserSeenIntro(Action<bool> action)
        {
            LocalStorage.HasUserSeenIntro(action);
        }

        public CategoryItem GetCategory(string categoryName)
        {
#if WINDOWS_PHONE
            var state = PhoneApplicationService.Current.State;
            
            object ocategory;
            if (state.TryGetValue(
                categoryName, 
                out ocategory)) return ocategory as CategoryItem;
#endif
            var category = this.Categories.Where(c => c.Name == categoryName).FirstOrDefault();
            category.LoadVideos();

#if WINDOWS_PHONE
            state[categoryName] = category;
#endif
            return category;
        }

        public void GetVideo(string category, string name, Action<VideoItem> result)
        {
            LocalStorage.GetVideo(category, name, vid =>
                {
                    if (vid != null)
                    {
                        result(vid);
                        return;
                    }

                    // didn't have the vid on disk, query the memory store.
                    vid = this.Categories
                        .Where(c => c.Name == category)
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
                
                CategoryItem.Initialize(this.Groups, this.Categories);
            }
        }

        public void TrackPageView(string title, string path)
        {
            path = path.Replace('/', '-').Replace(' ', '_');
            tracker.TrackPageView(title, path);
        }

        #region Private Methods

        private void StopQuerying()
        {
            // TODO: implement refcounting
            this.Querying = false;
            NotifyPropertyChanged("Querying");
        }

        private class QueryingHandle : IDisposable
        {
            private MainViewModel model;

            public QueryingHandle(MainViewModel model)
            {
                this.model = model;
            }

            void IDisposable.Dispose()
            {
                //this.model.StopQuerying();
            }
        }

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