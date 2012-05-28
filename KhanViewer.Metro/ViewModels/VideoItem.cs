using System;
using System.Runtime.Serialization;

namespace KhanViewer
{
    [DataContract]
    public class VideoItem : Item
    {
        [DataMember]
        public string Parent { get; set; }

        [DataMember]
        public string YoutubeId { get; set; }

        [DataMember]
        public Uri VideoUri { get; set; }

        [DataMember]
        public Uri VideoScreenshotUri { get; set; }

        [DataMember]
        public Uri VideoFileUri { get; set; }

        public void Navigate()
        {
            bool noVideoFileUri = this.VideoFileUri == null || string.IsNullOrWhiteSpace(this.VideoFileUri.ToString());

            if (noVideoFileUri)
            {
                Windows.System.Launcher.LaunchUriAsync(this.VideoUri);
            }
            else
            {
                Windows.System.Launcher.LaunchUriAsync(this.VideoFileUri);
            }
        }
    }
}
