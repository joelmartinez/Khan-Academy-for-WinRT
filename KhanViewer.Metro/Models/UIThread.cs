using System;

using Windows.UI.Core;
using Windows.UI.Popups;

namespace KhanViewer.Models
{
    public static class UIThread
    {
        private static CoreDispatcher Dispatcher;

        /// <summary>This should be called on the UI thread</summary>
        public static void Initialize(CoreDispatcher dispatcher)
        {
            Dispatcher = dispatcher;
        }

        /// <summary>
        ///   Invokes the given action on the UI thread - if the current thread is the UI thread this will just invoke the action directly on
        ///   the current thread so it can be safely called without the calling method being aware of which thread it is on.
        /// </summary>
        public static void Invoke(Action action)
        {
            if (Dispatcher == null)
            {
                action();
                return;
            }

            if (Dispatcher.HasThreadAccess)
                action.Invoke();
            else
                Dispatcher.Invoke(CoreDispatcherPriority.Normal, (s,a) => action(), Dispatcher, null);
        }

        public static void MessageBox(string message)
        {
            MessageDialog dialog = new MessageDialog(message);
            Invoke(() => dialog.ShowAsync());
        }

    }
}
