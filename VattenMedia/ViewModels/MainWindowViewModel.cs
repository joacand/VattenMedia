using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using VattenMedia.Core.Entities;
using System.Windows.Input;
using VattenMedia.Models;
using LiveChannel = VattenMedia.Models.LiveChannel;
using VattenMedia.Views;
using System.Timers;
using VattenMedia.Core.Interfaces;
using System.Linq;

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
        private static string ExampleUrl => "http://www.twitch.tv/channel";

        public ICommand LaunchCommand => new RelayCommand(OnLaunchCommand);
        public ICommand RefreshCommand => new RelayCommand(OnRefreshCommand);
        public ICommand LaunchFromUrlCommand => new RelayCommand(OnLaunchFromUrlCommand);
        public ICommand OAuthTwitchCommand => new RelayCommand(OnOAuthTwitchCommand);

        public ObservableCollection<LiveChannel> LiveChannels { get; private set; } = new ObservableCollection<LiveChannel>();
        public string UrlTextBox { get; set; } = ExampleUrl;
        public Quality SelectedQuality { get; set; } = Quality.High;
        public string StatusText { get => statusText; set => SetProperty(ref statusText, value); }

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
            Init();
        }

        private void Init()
        {
            streamStarterService.RunningProcessesChanged +=
                (s, e) => { RunningProcessesChangedHandler(e); };
            ListChannels();
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
