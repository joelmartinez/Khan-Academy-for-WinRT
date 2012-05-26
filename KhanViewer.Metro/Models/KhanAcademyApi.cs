using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;

namespace KhanViewer.Models
{

    public sealed class KhanAcademyApi : DataServer
    {
        protected override void LoadCategories(ObservableCollection<GroupItem> groups, ObservableCollection<CategoryItem> items, Action<CategoryItem[]> localSaveAction)
        {
            var queryHandle = App.ViewModel.StartQuerying();
            WebHelper.Json<JsonCategory[]>("http://www.khanacademy.org/api/v1/playlists", cats =>
            {
                using (queryHandle)
                {
                    // sort the playlists
                    var serverItems = cats
                        .Select(k => new CategoryItem 
                        { 
                            Name = k.Title, 
                            Description = k.Description, 
                            Slug = k.ExtendedSlug 
                        })
                        .OrderBy(k => k.Slug);

                    if (serverItems.Count() > 0)
                    {
                        // parse out the top level structures
                        var grouped = GroupItem.CreateGroups(serverItems);

                        // load the items up for the UI
                        UIThread.Invoke(() =>
                        {
                            groups.Clear();
                            foreach (var group in grouped) groups.Add(group);

                            items.Clear();
                            foreach (var item in serverItems) items.Add(item);
                        });

                        // save to disk
                        localSaveAction(serverItems.ToArray());
                    }
                    else
                    {
                        App.ViewModel.SetError("No Categories returned");
                    }
                }
            },
            e =>
            {
                App.ViewModel.SetError(e.Message);
            });
        }

        protected override void LoadVideos(string category, ObservableCollection<VideoItem> items, Action<VideoItem[]> localSaveAction)
        { 
            var queryHandle = App.ViewModel.StartQuerying();

            string apiUrl = string.Format("http://www.khanacademy.org/api/v1/playlists/{0}/videos", category);

            WebHelper.Json<JsonVideo[]>(apiUrl, vids =>
            {
                using (queryHandle)
                {
                    var serverItems = vids.Select(k => new VideoItem { 
                        Name = k.Title, 
                        Description = k.Description, 
                        YoutubeId = k.YouTubeId,
                        VideoUri = new Uri(k.Url),
                        VideoFileUri = k.Downloads != null ? new Uri(k.Downloads.Video) : null,
                        VideoScreenshotUri = k.Downloads != null ? new Uri(k.Downloads.Screenshot) : null,
                        Parent = category });
                    if (serverItems.Count() > 0)
                    {
                        UIThread.Invoke(() =>
                        {
                            items.Clear();
                            foreach (var item in serverItems)
                            {
                                items.Add(item);
                            }
                        });

                        localSaveAction(serverItems.ToArray());
                    }
                    else
                    {
                        App.ViewModel.SetError("No Videos returned for " + category);
                    }
                }
            },
            e =>
            {
                App.ViewModel.SetError(e.Message);
            });
        }

        [DataContract]
        public class JsonCategory
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

        }

        [DataContract]
        public class JsonDownloads
        {
            [DataMember(Name = "mp4")]
            public string Video { get; set; }
            [DataMember(Name = "png")]
            public string Screenshot { get; set; }
        }

        //http://www.khanacademy.org/api/v1/topictree
        /*    
    "description": "All concepts fit into the root of all knowledge", 
    "hide": true, 
    "id": "root", 
    "ka_url": "http://www.khanacademy.org#root", 
    "kind": "Topic", Url, Video
    "relative_url": "#root", 
    "standalone_title": "The Root of All Knowledge", 
    "tags": [], 
    "title": "The Root of All Knowledge"*/

        // children
        [DataContract]
        public class JsonTreeNode
        {
            public IEnumerable<JsonTreeNode> Children { get; set; }
        }
    }
}
