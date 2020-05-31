using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Data;
using VattenMedia.Core.Interfaces;
using VattenMedia.Models;

namespace VattenMedia.ViewModels
{
    internal class ChatViewModel : BaseViewModel
    {
        private readonly ITwitchChatClient twitchChatClient;
        private Thread chatPoller;
        private bool pollingActive;
        private string channelName;

        private readonly object chatMessagesLock = new object();
        private ObservableCollection<ChatMessage> chatMessages;
        private Dictionary<string, string> UsernameToColor { get; } = new Dictionary<string, string>();

        private static readonly string[] usernameColorOptions = new string[]
        {
            "#FF0000", "#0000FF", "#008000", "#B22222", "#FF7F50", "#9ACD32", "#FF4500",
            "#2E8B57", "#DAA520", "#5F9EA0", "#1E90FF", "#FF69B4", "#8A2BE2", "#00FF7F"
        };

        private Random Random { get; } = new Random();

        public ObservableCollection<ChatMessage> ChatMessages {
            get {
                return chatMessages;
            }
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
                string usernameColor = GetUsernameColor(username);
                var strippedMessage = ": " + Regex.Split(message, $"privmsg #{channelName} :", RegexOptions.IgnoreCase).Last();
                ChatMessages.Add(new ChatMessage { DateTime = DateTime.Now, Username = username, UsernameColor = usernameColor, Message = strippedMessage });
            }
        }

        private string GetUsernameColor(string username)
        {
            if (!UsernameToColor.ContainsKey(username))
            {
                var randColorInd = Random.Next(0, usernameColorOptions.Length);
                var newColor = usernameColorOptions[randColorInd];
                UsernameToColor.Add(username, newColor);
            }
            return UsernameToColor[username];
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
