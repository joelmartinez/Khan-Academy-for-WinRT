using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using System.Collections.ObjectModel;
using Windows.UI;
using System.Net.Http;
using Windows.Data.Json;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using Windows.ApplicationModel;
using Windows.Foundation;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.Foundation.Collections;
using Windows.UI.Xaml.Data;
using Windows.Storage.Streams;
using Windows.Storage;
using System.Xml;
using System.IO;


namespace KhanAcademy.Data
{
    [Windows.Foundation.Metadata.WebHostHidden]
    [DataContractAttribute ]
    public abstract class DataItem : KhanAcademy.Common.BindableBase
    {
        internal static Uri _baseUri = new Uri("ms-appx:///");
        private static ColorManager Colors = new ColorManager();

        public DataItem(String name, String description)
        {
            this._name = name;
            this._description = description;
        }

        private SolidColorBrush _color;

		[IgnoreDataMember]
        public SolidColorBrush Color
        {
            get { return this._color; }
            set { this.SetProperty(ref this._color, value); }
        }

        private int _colorIndex = 0;
        [DataMember]
        public int ColorIndex
        {
            get { return _colorIndex; }
            set
            {
                _color = Colors.GetColor(value);
                _colorIndex = value;
            }
        }

        /// <summary>This is mainly here to aid in binding to the standard templates, which expect a title property</summary>
        public string Title
        {
            get { return this.Name; }
        }

        private string _name = string.Empty;
		[DataMember]
        public string Name
        {
            get { return this._name; }
            set { this.SetProperty(ref this._name, value); }
        }

        private string _description = string.Empty;
		[DataMember]
        public string Description
        {
            get 
            {
                if (_description == string.Empty)
                {
                    _description =  "Videos covering " + this._name;
                }
                return this._description; 
            
            }
            set { this.SetProperty(ref this._description, value); }
        }

        public int AssignNextColor()
        {
            ColorIndex = Colors.AssignNextIndex();
            return _colorIndex;
        }

        /// <summary>Manage the pool of colors that are assigned to topics</summary>
        private class ColorManager
        {
            private List<Color> TopicColors = new List<Color>(){
                ColorHelper.FromArgb(140,34,46,61),
                ColorHelper.FromArgb(140,34,46,61),
                ColorHelper.FromArgb(255,131,168,37),
                ColorHelper.FromArgb(255,169,139,36),
                ColorHelper.FromArgb(255,168,37,37),
                ColorHelper.FromArgb(255,37,168,130),
                ColorHelper.FromArgb(255,37,108,168),
                ColorHelper.FromArgb(255,112,13,92)        
            };

            private int nextColor = 0;

            public int AssignNextIndex()
            {
                return nextColor++;
            }

            public SolidColorBrush GetColor(int index)
            {
                if (index >= TopicColors.Count)
                {
                    index = 0;
                    nextColor = 0;
                }

                return (new SolidColorBrush(TopicColors[index]));
            }
        }
    }

	[DataContractAttribute]
    public class VideoItem : DataItem
    {
        public VideoItem()
            : base(String.Empty, String.Empty)
        { }

        public VideoItem(String name, String description, String parent, String youTubeID, String videoPath, String imagePath, String filePath, SolidColorBrush color)
            : base(name, description)
        {
            this._parent = parent;
            this._youTubeID = youTubeID;
            this._videoPath = new Uri(videoPath);
            this._imagePath = new Uri(imagePath);
            this._filePath = new Uri(filePath);
            this.Color = color;
        }

        private string _parent = string.Empty;
		[DataMember]
        public string Parent
        {
            get { return this._parent; }
            set { this.SetProperty(ref this._parent, value); }
        }

        private string _youTubeID = string.Empty;
		[DataMember]
        public string YouTubeID
        {
            get { return this._youTubeID; }
            set { this.SetProperty(ref this._youTubeID, value); }
        }

        private Uri _videoPath = null;
		[DataMember]
        public Uri VideoPath
        {
            get { return this._videoPath; }
            set { this._videoPath = value; }
        }

        private Uri _filePath = null;
		[DataMember]
        public Uri FilePath
        {
            get { return this._filePath; }
			set { this._filePath = value; }
        }

		[DataMember]
        public Uri KhanPath { get; set; }

        private Uri _imagePath = null;
		[DataMember]
        public Uri ImagePath
        {
            get { return this._imagePath; }
            set { this._imagePath = value; }
        }

        private ImageSource _image;
		//[DataMember]
        public ImageSource Image
        {
            get
            {
                if (this._image == null && this._imagePath != null)
                {
                    this._image = new BitmapImage(this._imagePath);
                }
                return this._image;
            }
            set
            {
                this._imagePath = null;
                this.SetProperty(ref this._image, value);
            }
        }

        private DateTime _dateadded = DateTime.Now;
		[DataMember]
        public DateTime DateAdded
        {
            get
            {
                return this._dateadded;
            }
            set
            {
                this._dateadded = value;
                this.SetProperty(ref this._dateadded, value);
            }
        }

        public static VideoComparer CreateComparer()
        {
            return new VideoComparer();
        }

        public class VideoComparer : IEqualityComparer<VideoItem>
        {
            public bool Equals(VideoItem x, VideoItem y)
            {
                if (x == null && y == null) return true;
                if ((x == null && y != null) || (x != null && y == null)) return false;

                return x.Description == y.Description && x.Name == y.Name;
            }

            public int GetHashCode(VideoItem obj)
            {
                if (obj == null) return -1;

                var name = obj.Name ?? string.Empty;
                var desc = obj.Description ?? string.Empty;
                
                return name.GetHashCode() ^ desc.GetHashCode();
            }
        }
    }

	[DataContractAttribute]
    public class PlaylistItem : DataItem
    {
        public PlaylistItem()
            : base(String.Empty, String.Empty)
        { }

        public PlaylistItem(String name, String description)
            : base(name, description)
        { }

        private ObservableCollection<VideoItem> _videos = new ObservableCollection<VideoItem>();
		[DataMember]
        public ObservableCollection<VideoItem> Videos
        {
            get { return this._videos; }
			set { this._videos = value; }
        }

		[DataMember]
        public string Slug { get; set; }

        /// <summary>Used to simplify building the video list from the topic tree</summary>
        [IgnoreDataMember]
        public JsonNode SourceNode { get; set; }

        public string GroupKey
        {
            get
            {
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
                value = value.Replace(" and ", " & ");
                // Can we convert the text here so that it is formatted in "initial caps" (e.g. from "new & noteworthy" to "New & Noteworthy"? )
                return value;
            }
        }

        public int Count
        {
            get { return this.Videos.Count; }
        }

        public override string ToString()
        {
            return string.Format("{2} playlist: {0} ({1})", this.Name, this.Videos == null ? 0 : this.Videos.Count, this.Slug);
        }
    }

	[DataContract]
    public class TopicItem : DataItem
    {
        public TopicItem()
            : this(String.Empty, String.Empty)
        { }

        public TopicItem(String name, String description)
            : base(name, description)
        {
            // manipulate the preferred order of topics
            if (name.StartsWith("New")) this.Order = 0;
            if (name.StartsWith("Talks")) this.Order = 1;
            if (name.StartsWith("Math")) this.Order = 2;
            if (name.StartsWith("Science")) this.Order = 3;
            if (name.StartsWith("Test")) this.Order = 100; // end of the line
        }

        /// <summary>Used to set the primary order on the hub</summary>
		[DataMember]
        public int Order = 99;

        public IEnumerable<DataItem> HubContent
        {
            get 
            {
                if (this.ContentType == TopicContentType.Videolist)
                {
                    var list = this.Playlists[0].Videos
                        .Where(v => v.VideoPath != null)
                        .OrderByDescending(v => v.DateAdded);
                    return new ObservableCollection<DataItem>(list).Take(6);
                }
                else
                {
                    return new ObservableCollection<DataItem>(this.Playlists).Take(12);
                }
            }
        }

        private ObservableCollection<PlaylistItem> _playlists = new ObservableCollection<PlaylistItem>();
		[DataMember]
        public ObservableCollection<PlaylistItem> Playlists
        {
            get { return this._playlists; }
			set { this._playlists = value; }
        }

        //public IEnumerable<PlaylistItem> TopPlaylists
        //{
        //    // Provides a subset of the full items collection to bind to from a GroupedItemsPage
        //    // for better UX fluidity, since GridView will not virtualize large items collections.
        //    get { return this._playlists.Take(12); }
        //}

		[DataMember]
        public TopicContentType ContentType = TopicContentType.Playlist;

        public int Count
        {
            get { return this.Playlists.Count; }
        }

        public int VideosCount
        {
            get
            {
                int total = 0;
                foreach (PlaylistItem pi in this.Playlists)
                {
                    total += pi.Videos.Count;
                }
                return total;
            }
        }

        private IEnumerable<PlaylistItem> ListSetter
        {
            set
            {
                Playlists.Clear();
                foreach (var item in value) Playlists.Add(item);
            }
        }

        public static IEnumerable<TopicItem> CreateGroups(IEnumerable<PlaylistItem> ungroupedPlaylists)
        {
            var grouped = ungroupedPlaylists
                .GroupBy(i => i.GroupKey)
                .Select(g => new TopicItem(g.Key, g.Key + " description")
                {
                    ListSetter = g
                });

            var res = grouped
                .OrderBy(i => i.Order)
                .ThenByDescending(i => i.Playlists.Count );

            return res;
        }

        private static void SetTopicOrder(IEnumerable<TopicItem> grouped, string name, int ordervalue)
        {
            TopicItem topic = grouped.SingleOrDefault(t => t.Name.StartsWith(name));
            if (topic != null) 
                topic.Order = ordervalue;
        }

    }

    public sealed class KhanDataSource
    {
		private const int Megabyte = 1024 * 1024;

        private ObservableCollection<TopicItem> _topicGroups = new ObservableCollection<TopicItem>();
        public ObservableCollection<TopicItem> TopicGroups
        {
            get { return this._topicGroups; }
        }
        
        public TopicItem GetTopicGroup(string groupName)
        {
            var matches = this.TopicGroups.Where((group) => group.Name.Equals(groupName));
            if ( matches.Count() == 1) return matches.First();
            return null;
        }

        public DataItem GetPlaylist(string playlistName)
        {
            var matches = this.TopicGroups.SelectMany(group => group.Playlists).Where((playlist) => playlist.Name.Equals(playlistName));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

		public async Task LoadAllData()
		{
			await LoadCachedData();
			LoadRemoteData();
		}

		public async Task LoadCachedData()
		{
			// load the disk cache while we wait for the server to respond
			JsonNode cached = await ReadLocalCacheAsync<JsonNode>(@"cache\topictree.json", @"data\topictree.json");
			PopulateGroups(cached);
		}

        public async void LoadRemoteData()
        {
            // start the call to get the data from the remote API
            HttpClient client = new HttpClient();
            client.MaxResponseContentBufferSize = Megabyte * 20; // Read up to 20MB of Data, topic tree is 6MB at the time of this writing

            // don't use await so that this runs while we're loading the 
            // locally cached data from disk
            HttpResponseMessage response = await client.GetAsync(new Uri("http://www.khanacademy.org/api/v1/topictree"));

            WriteLocalCacheAsync(await response.Content.ReadAsStringAsync(), @"cache\topictree.json");
        }

        public static async void WriteLocalCacheAsync(string value, string filename)
        {
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFile file = await folder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
            using (var stream = await file.OpenStreamForWriteAsync())
			{
				using (var writer = new StreamWriter(stream))
				{
					writer.Write(value);
				}
			}
        }

        /// <summary>Attempts to read and deserialize the file from the local folder. If not found,
        /// will optionally try to read a file deployed with the package (think, initial loadout).</summary>
        public static async Task<T> ReadLocalCacheAsync<T>(string filename, string deployedfile = null) where T : class
        {
            try
            {
                var result = await ReadLocalCacheAsync<T>(filename, ApplicationData.Current.LocalFolder);

                if (result != null)
                {
                    return result;
                }
                else if (!string.IsNullOrWhiteSpace(deployedfile))
                {
                    return await ReadLocalCacheAsync<T>(deployedfile, Package.Current.InstalledLocation);
                }

                return default(T);
            }
            catch (Exception ex)
            {
                // something went wrong, just return a placeholder
                return default(T);
            }
        }

        private static async Task<T> ReadLocalCacheAsync<T>(string filename, StorageFolder folder) where T : class
        {
            try
            {
                var file = await folder.GetFileAsync(filename);
				string result = await FileIO.ReadTextAsync(file);

                var serializer = new DataContractJsonSerializer(typeof(T));
                var memStream = new MemoryStream(Encoding.UTF8.GetBytes(result));
                var serializedResult = serializer.ReadObject(memStream) as T;

                return serializedResult;
            }
            catch (FileNotFoundException)
            {
                return default(T);
            }
        }

        void PopulateGroups(JsonNode root)
        {
            if (root == null) return;

            Func<JsonNode, bool> videoClause = v => v.Kind == "Video";
            Func<JsonNode, bool> topicClause = c => c.Kind == "Topic";

            var playlists = root.Children // first level are the main topic groups
                .SelectMany(group => group.Children.Where(topicClause) // now flatten the list of playlists
                                        .Flatten(g => g.Children.Where(topicClause))
                                        .Select(k => new PlaylistItem
                                        {
                                            Name = k.Title,
                                            Description = k.Description,
                                            Slug = group.Title,
                                            SourceNode = k
                                        })
                                        .OrderBy(k => k.Slug));
            
            // add in the new and interviews playlists which get filtered previously
            // since they are only one level deep

            playlists = playlists.Union(root
                .Children
                .Where(c => c.Title.StartsWith("New") || c.Title.StartsWith("Talk"))
                .Select(k => new PlaylistItem
                    {
                        Name = k.Title,
                        Description = k.Description,
                        Slug = k.Title,
                        SourceNode = k
                    }))
                .ToArray();

            foreach (var playlist in playlists)
            {
                // now load all the videos
                var videos = playlist.SourceNode
                    .Children
                    .Flatten(v => v.Children);

                foreach (var video in videos.Where(videoClause).Select(v => new VideoItem
                    {
                         Name = v.Title,
                         Description = v.Description,
                         ImagePath = v.Downloads != null ? new Uri(v.Downloads.Screenshot) : null,
                         VideoPath = v.Downloads != null ? new Uri(v.Downloads.Video) : null,
                         KhanPath = new Uri(v.Url),
                         Parent = playlist.Name,
                         DateAdded = DateTime.Parse(v.DateAdded)
                    }))
                {
                    playlist.Videos.Add(video);
                }
            }

            if (playlists.Count() > 0)
            {
                SortGroups(TopicItem.CreateGroups(playlists.Where(p => p.Videos.Count > 0)));
            }
        }

        void PopulateGroups(JsonArray array)
        {
            ObservableCollection<PlaylistItem> flatPlaylists = new ObservableCollection<PlaylistItem>();

            foreach (var item in array)
            {
                PlaylistItem pi = new PlaylistItem();
                var obj = item.GetObject();

                foreach ( var key in obj.Keys)
                {
                    IJsonValue val;

                    if ( !obj.TryGetValue(key, out val))
                        continue;

                    if (val == null || val.ValueType == JsonValueType.Null) 
                        continue;

                    switch (key.ToLower())
                    {
                        case "title":
                            pi.Name = val.GetString();
                            break;
                        case "description":
                            pi.Description = val.GetString();
                            break;
                        case "extended_slug":
                            pi.Slug = val.GetString();
                            break;
                    }
                }

                flatPlaylists.Add(pi);
            }

            SortGroups(TopicItem.CreateGroups(flatPlaylists));

        }

        void SortGroups(IEnumerable<TopicItem> grouped)
        {
            TopicGroups.Clear();
            foreach (var group in grouped)
            {
                group.AssignNextColor();

                foreach (PlaylistItem playlistItem in group.Playlists)
                {
                    playlistItem.Color = group.Color;
                }

                if (group.Playlists.Count == 1)
                {
                    group.ContentType = TopicContentType.Videolist;
                }
                TopicGroups.Add(group);
            }
        }

    }


    [DataContract]
    public class JsonVideo
    {
        [DataMember(Name = "ka_url")]
        public string Url { get; set; }
        [DataMember(Name = "url")]
        public string YoutubeUrl { get; set; }
        [DataMember(Name = "title")]
        public string Title { get; set; }
        [DataMember(Name = "description")]
        public string Description { get; set; }
        [DataMember(Name = "youtube_id")]
        public string YouTubeId { get; set; }
        [DataMember(Name = "readable_id")]
        public string ReadableId { get; set; }
        [DataMember(Name = "keywords")]
        public string Keywords { get; set; }
        [DataMember(Name = "download_urls")]
        public JsonDownloads Downloads { get; set; }
        [DataMember(Name = "date_added")]
        public DateTime DateAdded { get; set; }

    }

    [DataContract]
    public class JsonDownloads
    {
        [DataMember(Name = "mp4")]
        public string Video { get; set; }
        [DataMember(Name = "png")]
        public string Screenshot { get; set; }
    }

    [DataContract]
    public class JsonPlaylist
    {
        [DataMember(Name = "ka_url")]
        public string Url { get; set; }
        [DataMember(Name = "url")]
        public string YoutubeUrl { get; set; }
        [DataMember(Name = "title")]
        public string Title { get; set; }
        [DataMember(Name = "description")]
        public string Description { get; set; }
        [DataMember(Name = "youtube_id")]
        public string YoutubeId { get; set; }
        [DataMember(Name = "extended_slug")]
        public string ExtendedSlug { get; set; }
    }

    [DataContract]
    public class JsonNode
    {
        [DataMember(Name = "children")]
        public JsonNode[] Children { get; set; }

        [DataMember(Name = "kind")]
        public string Kind { get; set; }

        [DataMember(Name = "ka_url")]
        public string Url { get; set; }
        [DataMember(Name = "title")]
        public string Title { get; set; }
        [DataMember(Name = "description")]
        public string Description { get; set; }

        #region playlist methods

        [DataMember(Name = "extended_slug")]
        public string ExtendedSlug { get; set; }

        #endregion

        #region video properties

        [DataMember(Name = "url")]
        public string YoutubeUrl { get; set; }
        [DataMember(Name = "youtube_id")]
        public string YouTubeId { get; set; }
        [DataMember(Name = "readable_id")]
        public string ReadableId { get; set; }
        [DataMember(Name = "keywords")]
        public string Keywords { get; set; }
        [DataMember(Name = "download_urls")]
        public JsonDownloads Downloads { get; set; }
        [DataMember(Name = "date_added")]
        public string DateAdded { get; set; }

        #endregion

        public override string ToString()
        {
            return string.Format("{0} - {1}", this.Kind, this.Title);
        }
    }
}
