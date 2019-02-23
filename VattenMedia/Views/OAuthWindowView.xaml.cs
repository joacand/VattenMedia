using System.Windows;
using VattenMedia.Core.Interfaces;
using VattenMedia.Infrastructure;
using VattenMedia.Infrastructure.Services;

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
            this.streamingService = streamingService;
            this.configHandler = configHandler;

            OAuthWebBrowser.Navigate(streamingService.OAuthUrl);
            OAuthWebBrowser.Navigated += OAuthWebBrowser_Navigated;
        }

        private void OAuthWebBrowser_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            string url = e.Uri.ToString();
            Navigating(url);
        }

        private void WebBrowser_NavigateError(object pDisp, ref object URL, ref object Frame, ref object StatusCode, ref bool Cancel)
        {
            Navigating((string)URL);
        }

        private async void Navigating(string url)
        {
            // Ignore non-redirect URL navigations
            if (url.Contains("twitch.tv") || url.Contains("google.com"))
            {
                return;
            }
            var authId = await streamingService.GetAuthIdFromUrl(url);
            configHandler.SetTwitchAccessToken(authId);
            Close();
        }
    }
}
