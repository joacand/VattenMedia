using System.Windows.Forms;
using VattenMedia.Infrastructure;

namespace VattenMedia
{
    public partial class OAuthWindow : Form
    {
        private readonly IStreamingService streamingService;
        public string AuthId { get; private set; } = "";

        public OAuthWindow(IStreamingService streamingService)
        {
            this.streamingService = streamingService;
            InitializeComponent();
        }

        public DialogResult Go()
        {
            // Hack to capture localhost navigation - this results in an error which doers not trigger Navigating but rather NavigateError
            OAuthWebBrowser.GoBack();
            var webBrowser = (SHDocVw.WebBrowser)OAuthWebBrowser.ActiveXInstance;
            webBrowser.NavigateError += WebBrowser_NavigateError;

            OAuthWebBrowser.Navigate(streamingService.OAuthUrl);
            DialogResult dialogResult = ShowDialog();
            return dialogResult;
        }

        private void WebBrowser_NavigateError(object pDisp, ref object URL, ref object Frame, ref object StatusCode, ref bool Cancel)
        {
            Navigating((string)URL);
        }

        private void OAuthWebBrowser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            string url = e.Url.ToString();
            Navigating(url);
        }

        private async void Navigating(string url)
        {
            // Ignore non-redirect URL navigations
            if (url.Contains("twitch.tv") || url.Contains("google.com"))
            {
                return;
            }

            AuthId = await streamingService.GetAuthIdFromUrl(url);

            if (!string.IsNullOrWhiteSpace(AuthId))
            {
                DialogResult = DialogResult.Yes;
            }
        }
    }
}
