using System;
using System.Windows;
using VattenMedia.ViewModels;

namespace VattenMedia.Views
{
    /// <summary>
    /// Interaction logic for ChatView.xaml
    /// </summary>
    public partial class ChatView : Window
    {
        private ChatViewModel Context => ((ChatViewModel)DataContext);

        public ChatView()
        {
            InitializeComponent();
        }

        internal void Initialize()
        {
            Closing += Context.OnWindowClosing;
        }

        internal void StartChat(string userName, string channelName, string twitchAccessToken)
        {
            Context.StartChat(userName, channelName, twitchAccessToken);
        }
    }
}
