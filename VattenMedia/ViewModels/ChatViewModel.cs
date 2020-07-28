using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;
using VattenMedia.Core.Entities;
using VattenMedia.Core.Interfaces;

namespace VattenMedia.ViewModels
{
    internal class ChatViewModel : BaseViewModel
    {
        private readonly ITwitchChatClient twitchChatClient;
        private readonly ITwitchEmotesService twitchEmotesService;
        private Thread chatPoller;
        private bool pollingActive;
        private string channelName;
        private string channelId;
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

        public ChatViewModel(ITwitchChatClient twitchChatClient, ITwitchEmotesService twitchEmotesService)
        {
            this.twitchChatClient = twitchChatClient;
            this.twitchEmotesService = twitchEmotesService;

            ChatMessages = new ObservableCollection<ChatMessage>();
        }

        public void StartChat(string userName, string channelName, string channelId, string twitchAccessToken)
        {
            SetTitle(channelName);
            this.channelName = channelName;
            this.channelId = channelId;
            Task.Run(async () => await twitchEmotesService.LoadChannelEmotes(channelId));
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
                    message = AddEmotes(message);
                    AddChatMessage(message);
                }
                catch (Exception ex)
                {
                    ChatMessages.Add(ChatMessage.ExceptionMessage(ex));
                }
            }
        }

        private string AddEmotes(string message)
        {
            var emoteResult = twitchEmotesService.CheckForEmotes(channelId, message);

            if (emoteResult.Any())
            ;

            return message;
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
