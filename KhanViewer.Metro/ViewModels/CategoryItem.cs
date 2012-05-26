using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using KhanViewer.Models;

#if !WINDOWS_PHONE
using Windows.UI;
#else
using System.Windows.Media;
#endif

namespace KhanViewer
{
    [DataContract]
    public class GroupItem
    {
        [DataMember]
        public string Name { get; set; }

        public Color Color { get; set; }

        public int Count
        {
            get
            {
                if (this.Playlists == null) return 0;
                return this.Playlists.Count;
            }
        }

        [DataMember]
        public ObservableCollection<CategoryItem> Playlists { get; set; }

        /// <summary>This member only exists to simplify adding playlists
        /// to the observable collection from the linq expression.</summary>
        private IEnumerable<CategoryItem> ListSetter
        {
            set
            {
                if (Playlists == null) Playlists = new ObservableCollection<CategoryItem>();
                else Playlists.Clear();

                foreach (var item in value) Playlists.Add(item);
            }
        }

        public static IEnumerable<GroupItem> CreateGroups(IEnumerable<CategoryItem> serverItems)
        {
            var grouped = serverItems
                .GroupBy(i => i.GroupKey)
                .Select(g => new GroupItem
                {
                    Name = g.Key,
                    Color = AssignNextColor(),
                    ListSetter = g
                });
            return grouped;
        }

        private static Color AssignNextColor()
        {
            // TODO: write color array and logic to assign colors
#if !WINDOWS_PHONE
            return ColorHelper.FromArgb(150, 150, 150, 150);
#else
            return Color.FromArgb(150, 150, 150, 150);
#endif
        }
    }

    [DataContract]
    public class CategoryItem : Item
    {
        private bool loaded = false;

        public CategoryItem()
        {
            this.Videos = new ObservableCollection<VideoItem>();
        }

        /// <summary>List of videos in this category</summary>
        [DataMember]
        public ObservableCollection<VideoItem> Videos { get; set; }

        [DataMember]
        public string Slug { get; set; }

        /// <summary>This is the top level group</summary>
        /// <remarks>Parsed from the first element in the slug</remarks>
        public string GroupKey
        {
            get
            {
                // make sure not empty
                if (string.IsNullOrWhiteSpace(this.Slug)) return string.Empty;

                string value;

                if (!this.Slug.Contains("/"))
                {
                    value = this.Slug;
                }
                else
                {
                    value = this.Slug.Substring(0, this.Slug.IndexOf("/"));
                }

                value = value.Replace("-", " ");

                return value;
            }
        }

        public void LoadVideos()
        {
            if (!loaded)
            {
                // first load what I know (ie. from disk)
                LocalStorage.GetVideos(this.Name, vids =>
                    {
                        UIThread.Invoke(() => { foreach (var vid in vids) Videos.Add(vid); });

                        loaded = true;
                    });

                // now kick off the server to the query
                Clouds.GetVideosFromServer(this.Videos, this.Name);
            }
            else if (this.Videos.Count == 0)
            {
                // if we've already loaded, but don't have any results, then need to try again
                Clouds.GetVideosFromServer(this.Videos, this.Name);
            }
        }

        public static void Initialize(ObservableCollection<GroupItem> groups, ObservableCollection<CategoryItem> items)
        {
            // first load what I know
            LocalStorage.GetCategories(playlists =>
                {
                    var grouped = GroupItem.CreateGroups(playlists);

                    UIThread.Invoke(() => 
                    {
                        groups.Clear();
                        items.Clear();
                        foreach (var group in grouped) groups.Add(group);
                        foreach (var list in playlists) items.Add(list); 
                    });

                    // then start to query the server
                    Clouds.LoadCategoriesFromServer(groups, items);
                });
        }
    }
}
