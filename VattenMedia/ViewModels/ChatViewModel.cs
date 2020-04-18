using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Data;
using VattenMedia.Core.Interfaces;

namespace VattenMedia.ViewModels
{
    internal class ChatViewModel : BaseViewModel
    {
        private readonly ITwitchChatClient twitchChatClient;
        private Thread chatPoller;
        private bool pollingActive;
        private string channelName;

        private readonly object _chatMessagesLock = new object();
        private ObservableCollection<string> _chatMessages;

        public ObservableCollection<string> ChatMessages {
            get {
                return _chatMessages;
            }
            set {
                _chatMessages = value;
                BindingOperations.EnableCollectionSynchronization(_chatMessages, _chatMessagesLock);
            }
        }

        public ChatViewModel(ITwitchChatClient twitchChatClient)
        {
            this.twitchChatClient = twitchChatClient;

            ChatMessages = new ObservableCollection<string>();
        }

        public void StartChat(string userName, string channelName, string twitchAccessToken)
        {
            this.channelName = channelName;
            ChatMessages.Clear();
            twitchChatClient.Start(userName, twitchAccessToken, channelName);

            chatPoller = new Thread(new ThreadStart(PollChat))
            {
                IsBackground = true
            };
            chatPoller.Start();
        }

        private void PollChat()
        {
            pollingActive = true;
            while (pollingActive)
            {
                var message = twitchChatClient.ReadMessage();
                AddChatMessage(message);
            }
        }

        private void AddChatMessage(string message)
        {
            if (message.Contains($"privmsg #{channelName}", StringComparison.OrdinalIgnoreCase))
            {
                var username = message.Split("!").First().Substring(1);
                var strippedMessage = Regex.Split(message, $"privmsg #{channelName}", RegexOptions.IgnoreCase).Last();
                ChatMessages.Add($"{username}: {strippedMessage}");
            }
        }

        internal void OnWindowClosing(object sender, CancelEventArgs e)
        {
            StopChat();
        }

        private void StopChat()
        {
            pollingActive = false;
            twitchChatClient.Stop();
        }
    }
}
