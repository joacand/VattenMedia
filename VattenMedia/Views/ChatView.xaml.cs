using System.Windows;
using System.Windows.Controls;
using VattenMedia.ViewModels;

namespace VattenMedia.Views
{
    /// <summary>
    /// Interaction logic for ChatView.xaml
    /// </summary>
    public partial class ChatView : Window
    {
        private ChatViewModel Context => ((ChatViewModel)DataContext);
        private bool AutoScroll = true;

        public ChatView()
        {
            InitializeComponent();
        }

        internal void Initialize()
        {
            Closing += Context.OnWindowClosing;
        }

        internal void StartChat(string userName, string channelName, string channelId, string twitchAccessToken)
        {
            Context.StartChat(userName, channelName, channelId, twitchAccessToken);
        }

        private void ScrollViewer_ScrollChanged(object _, ScrollChangedEventArgs e)
        {
            if (e?.Source as ScrollViewer == null)
            {
                return;
            }

            if (e.ExtentHeightChange == 0)
            {
                if ((e.Source as ScrollViewer).VerticalOffset == (e.Source as ScrollViewer).ScrollableHeight)
                {
                    AutoScroll = true;
                }
                else
                {
                    AutoScroll = false;
                }
            }

            if (AutoScroll && e.ExtentHeightChange != 0)
                (e.Source as ScrollViewer).ScrollToVerticalOffset((e.Source as ScrollViewer).ExtentHeight);
        }
    }
}
