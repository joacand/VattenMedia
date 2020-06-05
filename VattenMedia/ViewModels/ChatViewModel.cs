using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Windows.Data;
using VattenMedia.Core.Entities;
using VattenMedia.Core.Interfaces;

namespace VattenMedia.ViewModels
{
    internal class ChatViewModel : BaseViewModel
    {
        private readonly ITwitchChatClient twitchChatClient;
        private Thread chatPoller;
        private bool pollingActive;
        private string channelName;
        private string chatViewTitle;
        private ObservableCollection<ChatMessage> chatMessages;
        private readonly object chatMessagesLock = new object();

        public string ChatViewTitle { get => chatViewTitle; set => SetProperty(ref chatViewTitle, value); }

        public ObservableCollection<ChatMessage> ChatMessages {
            get => chatMessages;
            set {
                chatMessages = value;
                BindingOperations.EnableCollectionSynchronization(chatMessages, chatMessagesLock);
            }
        }

        public ChatViewModel(ITwitchChatClient twitchChatClient)
        {
            this.twitchChatClient = twitchChatClient;

            ChatMessages = new ObservableCollection<ChatMessage>();
        }

        public void StartChat(string userName, string channelName, string twitchAccessToken)
        {
            SetTitle(channelName);
            this.channelName = channelName;
            ChatMessages.Clear();
            twitchChatClient.Start(userName, twitchAccessToken, channelName);

            chatPoller = new Thread(new ThreadStart(PollChat))
            {
                IsBackground = true
            };
            chatPoller.Start();
        }

        internal void OnWindowClosing(object sender, CancelEventArgs e)
        {
            StopChat();
        }

        private void SetTitle(string channelName)
        {
            ChatViewTitle = $"Chat - {channelName}";
        }

        private void PollChat()
        {
            pollingActive = true;
            while (pollingActive)
            {
                try
                {
                    var message = twitchChatClient.ReadMessage();
                    AddChatMessage(message);
                }
                catch (Exception ex)
                {
                    ChatMessages.Add(ChatMessage.ExceptionMessage(ex));
                }
            }
        }

        private void AddChatMessage(string message)
        {
            var formattedMessage = twitchChatClient.FormatMessage(message, channelName);
            if (!formattedMessage.IsEmpty)
            {
                ChatMessages.Add(formattedMessage);
            }
        }

        private void StopChat()
        {
            pollingActive = false;
            twitchChatClient.Stop();
        }
    }
}
