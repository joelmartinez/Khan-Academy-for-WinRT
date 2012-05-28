using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using Windows.UI;

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
        public ObservableCollection<PlaylistItem> Playlists { get; set; }

        /// <summary>This member only exists to simplify adding playlists
        /// to the observable collection from the linq expression.</summary>
        private IEnumerable<PlaylistItem> ListSetter
        {
            set
            {
                if (Playlists == null) Playlists = new ObservableCollection<PlaylistItem>();
                else Playlists.Clear();

                foreach (var item in value) Playlists.Add(item);
            }
        }

        public static IEnumerable<GroupItem> CreateGroups(IEnumerable<PlaylistItem> serverItems)
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
            return ColorHelper.FromArgb(150, 150, 150, 150);
        }
    }
}
