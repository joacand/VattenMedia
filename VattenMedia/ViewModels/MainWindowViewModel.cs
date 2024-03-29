﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Controls;
using System.Windows.Input;
using VattenMedia.Core.Entities;
using VattenMedia.Core.Interfaces;
using VattenMedia.Factories;
using VattenMedia.Models;
using VattenMedia.Views;
using LiveChannel = VattenMedia.Models.LiveChannel;
using Video = VattenMedia.Models.Video;

namespace VattenMedia.ViewModels
{
    internal class MainWindowViewModel : BaseViewModel
    {
        private readonly IViewModelFactory viewModelFactory;
        private readonly IConfigHandler configHandler;
        private readonly ITwitchService twitchService;
        private readonly IYoutubeService youtubeService;
        private readonly IStatusTextService statusManager;
        private readonly IStreamStarterService streamStarterService;
        private readonly ILogger logger;
        private readonly AppConfiguration appConfiguration;
        private int runningProcesses;
        private Timer statusTextTimer;
        private string statusText;
        private UserControl streamContentControl;
        private static string ExampleUrl => "https://www.twitch.tv/channel";

        public ICommand LaunchCommand => new RelayCommand(OnLaunchCommand);
        public ICommand RefreshCommand => new RelayCommand(OnRefreshCommand);
        public ICommand LaunchFromUrlCommand => new RelayCommand(OnLaunchFromUrlCommand);
        public ICommand OAuthTwitchCommand => new RelayCommand(OnOAuthTwitchCommand);
        public ICommand ChangeToListViewCommand => new RelayCommand(OnChangeToListViewCommand);
        public ICommand ChangeToGridViewCommand => new RelayCommand(OnChangeToGridViewCommand);
        public ICommand AddToFavoritesCommand => new RelayCommand(OnAddToFavoritesCommand);
        public ICommand OpenVideosForChannelCommand => new RelayCommand(OnOpenVideosForChannelCommand);
        public ICommand OpenVideosCommand => new RelayCommand(OnOpenVideosCommand);
        public ICommand OpenChatCommand => new RelayCommand(OnOpenChatCommand);

        public ObservableCollection<LiveChannel> LiveChannels { get; private set; } = new ObservableCollection<LiveChannel>();
        public ObservableCollection<Video> ChannelVideos { get; private set; } = new ObservableCollection<Video>();
        public string UrlTextBox { get; set; } = ExampleUrl;
        public Quality SelectedQuality { get; set; } = Quality.High;
        public string StatusText { get => statusText; set => SetProperty(ref statusText, value); }
        public UserControl StreamContentControl { get => streamContentControl; set => SetProperty(ref streamContentControl, value); }
        public UserControl StreamGridControl { get; set; }
        public UserControl StreamListControl { get; set; }
        public UserControl VideoListControl { get; set; }

        public MainWindowViewModel(
            IViewModelFactory viewModelFactory,
            IConfigHandler configHandler,
            ITwitchService twitchService,
            IYoutubeService youtubeService,
            IStatusTextService statusManager,
            IStreamStarterService streamStarterService,
            ILogger logger,
            AppConfiguration appConfiguration)
        {
            this.viewModelFactory = viewModelFactory;
            this.configHandler = configHandler;
            this.twitchService = twitchService;
            this.youtubeService = youtubeService;
            this.statusManager = statusManager;
            statusManager.SetCallback(ChangeStatusText);
            this.streamStarterService = streamStarterService;
            this.logger = logger;
            this.appConfiguration = appConfiguration;
            streamStarterService.RunningProcessesChanged +=
                (s, e) => { RunningProcessesChangedHandler(e); };
            ListChannels();
        }

        public void Initialize()
        {
            LoadViewType();
        }

        private void LoadViewType()
        {
            var viewType = configHandler.GetViewType();
            switch (viewType)
            {
                case ViewType.List:
                    OnChangeToListViewCommand();
                    break;
                case ViewType.Grid:
                    OnChangeToGridViewCommand();
                    break;
            }
        }

        private void RunningProcessesChangedHandler(int processes)
        {
            runningProcesses = processes;
            UpdateRunningProcessesText();
        }

        private void UpdateRunningProcessesText()
        {
            if (runningProcesses == 0)
            {
                ChangeStatusText("");
            }
            else
            {
                ChangeStatusText($"Running processes: {runningProcesses}");
            }
        }

        private void ChangeStatusText(string status, TimeSpan? time = null, bool isError = false)
        {
            if (isError)
            {
                logger.LogError(status);
            }

            if (statusTextTimer != null && statusTextTimer.Enabled && time != null)
            {
                statusTextTimer.Stop();
            }

            StatusText = status;

            if (time != null)
            {
                if (statusTextTimer == null)
                {
                    statusTextTimer = new Timer();
                }
                statusTextTimer.Elapsed += (sender, args) => UpdateRunningProcessesText();
                statusTextTimer.Interval = time.Value.TotalMilliseconds;
                statusTextTimer.AutoReset = false;
                statusTextTimer.Start();
            }
        }

        private async void ListVideos(string channelId)
        {
            ChannelVideos.Clear();
            try
            {
                var videos = await twitchService.GetVideos(configHandler.Config.TwitchAccessToken, channelId);
                foreach (var video in videos.OrderByDescending(x => x.PublishedAt))
                {
                    ChannelVideos.Add(new Video(video.Name, video.Title, video.Game, video.Length, video.BitmapUrl, video.Url, video.PublishedAt));
                }
            }
            catch (Exception e)
            {
                var errorMessage = $"Error: Failed to get videos from Twitch. Exception: {e}";
                statusManager.ChangeStatusText(errorMessage);
                logger.LogError(errorMessage);
            }
            ChangeToVideoListView();
        }

        private void ListChannels()
        {
            if (appConfiguration.TwitchEnabled && configHandler.HasTwitchAccessToken)
            {
                ListChannels(twitchService, configHandler.Config.TwitchAccessToken);
            }
            if (appConfiguration.YoutubeEnabled && configHandler.HasYoutubeAccessToken)
            {
                ListChannels(youtubeService, configHandler.Config.YoutubeToken);
            }
        }

        private async void ListChannels(IStreamingService streamingService, string accessToken)
        {
            LiveChannels.Clear();
            try
            {
                var channels = await streamingService.GetLiveChannels(accessToken);
                foreach (var channel in channels
                    .OrderByDescending(x => configHandler.IsFavorited(x.Name))
                    .ThenByDescending(x => x.Viewers))
                {
                    var liveChannel = new LiveChannel(channel, configHandler.IsFavorited(channel.Name));
                    LiveChannels.Add(liveChannel);
                    _ = Task.Run(() =>
                    {
                        liveChannel.LoadImage();
                        liveChannel.OnPropertyChanged(nameof(liveChannel.Image));
                    });
                }
            }
            catch (Exception e)
            {
                var errorMessage = $"Error: Failed to get live channels from Twitch. Exception: {e}";
                statusManager.ChangeStatusText(errorMessage);
                logger.LogError(errorMessage);
            }
        }

        private void OnLaunchCommand(object uri)
        {
            try
            {
                StartStream((Uri)uri);
            }
            catch (Exception ex)
            {
                ChangeStatusText($"Failed to launch stream - {ex}", isError: true);
            }
        }

        private void OnLaunchFromUrlCommand(object _)
        {
            if (UrlTextBox.Equals(ExampleUrl))
            {
                return;
            }
            try
            {
                StartStream(new Uri(UrlTextBox));
            }
            catch (Exception ex)
            {
                ChangeStatusText($"Failed to launch from URL - {ex}", isError: true);
            }
        }

        private void OnOAuthTwitchCommand(object _)
        {
            if (string.IsNullOrEmpty(configHandler.Config.TwitchClientId))
            {
                ChangeStatusText("Client id missing - cannot authenticate", isError: true);
            }
            else
            {
                var oAuthWindow = new OAuthWindowView(twitchService, configHandler);
                oAuthWindow.ShowDialog();
                OnRefreshCommand();
            }
        }

        private void OnChangeToListViewCommand(object _ = null)
        {
            StreamContentControl = StreamListControl;
            configHandler.SetViewType(ViewType.List);
        }

        private void OnChangeToGridViewCommand(object _ = null)
        {
            StreamContentControl = StreamGridControl;
            configHandler.SetViewType(ViewType.Grid);
        }

        private void ChangeToVideoListView()
        {
            StreamContentControl = VideoListControl;
        }

        private void OnAddToFavoritesCommand(object selectedItem)
        {
            if (selectedItem != null && selectedItem is LiveChannel channel)
            {
                var isFavorited = configHandler.ToggleFavorited(channel.Name);
                channel.IsFavorited = isFavorited;
                RefreshChannelsUi();
            }
        }

        private void OnOpenVideosForChannelCommand(object selectedItem)
        {
            if (selectedItem != null && selectedItem is LiveChannel channel)
            {
                ListVideos(channel.ChannelId);
            }
        }

        private void OnOpenVideosCommand(object _)
        {
            if (!UrlTextBox.Equals(ExampleUrl))
            {
                var channelName = UrlTextBox.Contains("/")
                    ? UrlTextBox.Split("/").Last()
                    : UrlTextBox;
                ListVideosByChannelName(channelName);
            }
        }

        private void OnOpenChatCommand(object selectedItem)
        {
            if (selectedItem != null && selectedItem is LiveChannel channel)
            {
                OpenChat(channel.Name);
            }
        }

        private void OpenChat(string channelName)
        {
            if (string.IsNullOrEmpty(configHandler.Config.TwitchAccessToken))
            {
                ChangeStatusText("Access token missing - cannot open chat", isError: true);
                return;
            }
            if (string.IsNullOrEmpty(configHandler.Config.TwitchUsername))
            {
                ChangeStatusText("Username is missing - cannot open chat", isError: true);
                return;
            }
            var chatWindow = new ChatView { DataContext = viewModelFactory.CreateChatViewModel() };
            chatWindow.Initialize();
            chatWindow.StartChat(configHandler.Config.TwitchUsername, channelName, configHandler.Config.TwitchAccessToken);
            chatWindow.Show();
        }

        private async void ListVideosByChannelName(string channelName)
        {
            var channelId = await twitchService.GetChannelId(configHandler.Config.TwitchAccessToken, channelName);
            ListVideos(channelId);
        }

        private void OnRefreshCommand(object _ = null)
        {
            ListChannels();
        }

        private void RefreshChannelsUi()
        {
            var channels = new List<LiveChannel>(LiveChannels);
            LiveChannels.Clear();
            foreach (var channel in channels
                .OrderByDescending(x => configHandler.IsFavorited(x.Name))
                .ThenByDescending(x => x.Viewers))
            {
                LiveChannels.Add(channel);
            }
        }

        private void StartStream(Uri url)
        {
            streamStarterService.StartStream(url, GetQualityFromRadioButtons());
        }

        private List<string> GetQualityFromRadioButtons()
        {
            return SelectedQuality switch
            {
                Quality.High => QualityOptions.HighQuality,
                Quality.Medium => QualityOptions.MediumQuality,
                Quality.Low => QualityOptions.LowQuality,
                _ => throw new ArgumentOutOfRangeException(nameof(SelectedQuality)),
            };
        }
    }
}
