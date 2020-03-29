using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Timers;
using System.Windows.Controls;
using System.Windows.Input;
using VattenMedia.Core.Entities;
using VattenMedia.Core.Interfaces;
using VattenMedia.Models;
using VattenMedia.Views;
using LiveChannel = VattenMedia.Models.LiveChannel;

namespace VattenMedia.ViewModels
{
    internal class MainWindowViewModel : BaseViewModel
    {
        private readonly IConfigHandler configHandler;
        private readonly ITwitchService twitchService;
        private readonly IYoutubeService youtubeService;
        private readonly IStatusManager statusManager;
        private readonly IStreamStarterService streamStarterService;
        private readonly AppConfiguration appConfiguration;
        private int runningProcesses;
        private Timer statusTextTimer;
        private string statusText;
        private UserControl streamContentControl;
        private static string ExampleUrl => "http://www.twitch.tv/channel";

        public ICommand LaunchCommand => new RelayCommand(OnLaunchCommand);
        public ICommand RefreshCommand => new RelayCommand(OnRefreshCommand);
        public ICommand LaunchFromUrlCommand => new RelayCommand(OnLaunchFromUrlCommand);
        public ICommand OAuthTwitchCommand => new RelayCommand(OnOAuthTwitchCommand);
        public ICommand ChangeToListViewCommand => new RelayCommand(OnChangeToListViewCommand);
        public ICommand ChangeToGridViewCommand => new RelayCommand(OnChangeToGridViewCommand);


        public ObservableCollection<LiveChannel> LiveChannels { get; private set; } = new ObservableCollection<LiveChannel>();
        public string UrlTextBox { get; set; } = ExampleUrl;
        public Quality SelectedQuality { get; set; } = Quality.High;
        public string StatusText { get => statusText; set => SetProperty(ref statusText, value); }
        public UserControl StreamContentControl { get { return streamContentControl; } set => SetProperty(ref streamContentControl, value); }
        public UserControl StreamGridControl { get; set; }
        public UserControl StreamListControl { get; set; }

        public MainWindowViewModel(
            IConfigHandler configHandler,
            ITwitchService twitchService,
            IYoutubeService youtubeService,
            IStatusManager statusManager,
            IStreamStarterService streamStarterService,
            AppConfiguration appConfiguration)
        {
            this.configHandler = configHandler;
            this.twitchService = twitchService;
            this.youtubeService = youtubeService;
            this.statusManager = statusManager;
            statusManager.SetCallback(ChangeStatusText);
            this.streamStarterService = streamStarterService;
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

        private void ChangeStatusText(string status, TimeSpan? time = null)
        {
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
                foreach (var channel in channels.OrderByDescending(x => x.Viewers))
                {
                    LiveChannels.Add(new LiveChannel(channel));
                }
            }
            catch (Exception e)
            {
                statusManager.ChangeStatusText($"Error: Failed to get live channels from Twitch. Exception: {e}");
            }
        }

        private void OnLaunchCommand(object uri)
        {
            StartStream((Uri)uri);
        }

        private void OnLaunchFromUrlCommand(object _)
        {
            if (!UrlTextBox.Equals(ExampleUrl))
            {
                StartStream(new Uri(UrlTextBox));
            }
        }

        private void OnOAuthTwitchCommand(object _)
        {
            if (string.IsNullOrEmpty(configHandler.Config.TwitchClientId))
            {
                ChangeStatusText("Client id missing - cannot authenticate");
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

        private void OnRefreshCommand(object _ = null)
        {
            ListChannels();
        }

        private void StartStream(Uri url)
        {
            streamStarterService.StartStream(url, GetQualityFromRadioButtons());
        }

        private List<string> GetQualityFromRadioButtons()
        {
            switch (SelectedQuality)
            {
                case Quality.High:
                    return QualityOptions.HighQuality;
                case Quality.Medium:
                    return QualityOptions.MediumQuality;
                case Quality.Low:
                    return QualityOptions.LowQuality;
            }
            throw new ArgumentOutOfRangeException(nameof(SelectedQuality));
        }
    }
}
