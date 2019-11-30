using System;
using System.Windows;
using VattenMedia.Core.Interfaces;

namespace VattenMedia.Views
{
    /// <summary>
    /// Interaction logic for OAuthWindowView.xaml
    /// 
    /// TODO: Refactor out logic to ViewModel
    /// </summary>
    public partial class OAuthWindowView : Window
    {
        private readonly IStreamingService streamingService;
        private readonly IConfigHandler configHandler;

        public OAuthWindowView(IStreamingService streamingService, IConfigHandler configHandler)
        {
            InitializeComponent();
            this.streamingService = streamingService ?? throw new ArgumentNullException(nameof(streamingService));
            this.configHandler = configHandler ?? throw new ArgumentNullException(nameof(configHandler));

            OAuthWebBrowser.Navigate(streamingService.OAuthUrl);
            OAuthWebBrowser.Navigated += OAuthWebBrowser_Navigated;
        }

        private void OAuthWebBrowser_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            string url = e.Uri.ToString();
            Navigating(url);
        }

        private async void Navigating(string url)
        {
            // Ignore non-redirect URL navigations
            if (url.Contains("twitch.tv", StringComparison.OrdinalIgnoreCase) || url.Contains("google.com", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }
            var authId = await streamingService.GetAuthIdFromUrl(url);
            configHandler.SetTwitchAccessToken(authId);
            Close();
        }
    }
}
