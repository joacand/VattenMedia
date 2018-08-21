using System;
using System.Windows.Forms;
using VattenMedia.Infrastructure;

namespace VattenMedia
{
    public partial class OAuthWindow : Form
    {
        public string AuthId { get; private set; } = "";

        public OAuthWindow()
        {
            InitializeComponent();
        }

        public DialogResult Go(IStreamingService twitch)
        {
            OAuthWebBrowser.Navigate(twitch.OAuthUrl);
            DialogResult dialogResult = ShowDialog();
            return dialogResult;
        }

        private void OAuthWebBrowser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            string url = e.Url.ToString();

            if (url.Contains("access_token"))
            {
                AuthId = url.Split(new string[] { "access_token=" }, StringSplitOptions.None)[1].Split(new string[] { "&" }, StringSplitOptions.None)[0];
                DialogResult = DialogResult.Yes;
            }
        }
    }
}
