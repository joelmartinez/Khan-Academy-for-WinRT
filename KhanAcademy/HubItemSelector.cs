using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KhanAcademy.Data;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace KhanAcademy
{
    public class HubItemSelector : DataTemplateSelector
    {
        public DataTemplate Video { get; set; }
        public DataTemplate Playlist { get; set; }
        public DataTemplate Topic { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            //return null;

            DataItem dataItem = item as DataItem;

            if (dataItem.GetType() == typeof(VideoItem))
            {
                return Playlist;
            }
            else
            {
                return Topic;
            }
        }
    }
}
