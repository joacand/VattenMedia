using System.Windows.Forms;
using VattenMedia.Infrastructure;

namespace VattenMedia
{
    public partial class OAuthWindow : Form
    {
        private IStreamingService streamingService;
        public string AuthId { get; private set; } = "";

        public OAuthWindow(IStreamingService streamingService)
        {
            this.streamingService = streamingService;
            InitializeComponent();
        }

        public DialogResult Go()
        {
            OAuthWebBrowser.Navigate(streamingService.OAuthUrl);
            DialogResult dialogResult = ShowDialog();
            return dialogResult;
        }

        private void OAuthWebBrowser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            string url = e.Url.ToString();
            AuthId = streamingService.GetAuthIdFromUrl(url);

            if (!string.IsNullOrWhiteSpace(AuthId))
            {
                DialogResult = DialogResult.Yes;
            }
        }
    }
}
