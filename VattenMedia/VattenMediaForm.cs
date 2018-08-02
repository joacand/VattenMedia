using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using VattenMedia.Entities;
using VattenMedia.Infrastructure;
using Timer = System.Timers.Timer;

namespace VattenMedia
{
    public partial class VattenMediaForm : Form
    {
        private readonly IConfigHandler configHandler;
        private ITwitchService twitch;
        private readonly IStatusManager statusManager;
        private readonly IStreamService streamService;

        private List<TwitchChannel> channels = new List<TwitchChannel>();
        private Timer statusTextTimer;
        private int runningProcesses = 0;

        public VattenMediaForm()
        {
            InitializeComponent();

            configHandler = new ConfigHandler();
            twitch = new TwitchService(configHandler);
            statusManager = new StatusManager(ChangeStatusText);
            streamService = new StreamService(statusManager);

            Init();
        }

        public void UpdateColorControls(Control myControl)
        {
            myControl.BackColor = Color.FromArgb(57, 46, 92);
            myControl.ForeColor = Color.White;
            if (myControl is Button)
            {
                myControl.BackColor = Color.FromArgb(23, 20, 31);
                ((Button)myControl).TabStop = false;
                ((Button)myControl).FlatStyle = FlatStyle.Flat;
                ((Button)myControl).FlatAppearance.BorderSize = 0;
                ((Button)myControl).FlatAppearance.MouseDownBackColor = Color.FromArgb(116, 104, 148);
                ((Button)myControl).FlatAppearance.MouseOverBackColor = Color.FromArgb(116, 104, 148);
            }
            foreach (Control subC in myControl.Controls)
            {
                UpdateColorControls(subC);
            }
        }

        private void Init()
        {
            streamService.RunningProcessesChanged +=
                (s, e) => { RunningProcessesChangedHandler(e); };

            BackColor = Color.FromArgb(57, 46, 92);
            ForeColor = Color.White;
            foreach (Control c in Controls)
            {
                UpdateColorControls(c);
            }
            ChannelGridView.Font = new Font("Arial", 11);

            ChannelGridView.ColumnCount = 5;
            ChannelGridView.Columns[0].Name = "Channel";
            ChannelGridView.Columns[1].Name = "Title";
            ChannelGridView.Columns[2].Name = "Game";
            ChannelGridView.Columns[3].Name = "Viewers";
            ChannelGridView.Columns[4].Name = "Runtime";
            DataGridViewImageColumn imageCol = new DataGridViewImageColumn();
            ChannelGridView.Columns.Add(imageCol);

            foreach (DataGridViewColumn column in ChannelGridView.Columns)
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                column.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            }
            ChannelGridView.Columns[4].DefaultCellStyle.WrapMode = DataGridViewTriState.False;
            ChannelGridView.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            DataGridViewButtonColumn Launch = new DataGridViewButtonColumn
            {
                HeaderText = "Launch",
                Text = "Launch",
                UseColumnTextForButtonValue = true,
                FlatStyle = FlatStyle.Popup,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    SelectionBackColor = ChannelGridView.DefaultCellStyle.BackColor,
                    SelectionForeColor = ChannelGridView.DefaultCellStyle.ForeColor,
                    BackColor = ChannelGridView.DefaultCellStyle.BackColor,
                    ForeColor = ChannelGridView.DefaultCellStyle.ForeColor
                }
            };

            ChannelGridView.Columns.Add(Launch);
            SetAccessTokenLabel();
            ListChannels();
        }

        private void SetAccessTokenLabel()
        {
            if (configHandler.HasAccessToken)
            {
                OAuthAvailableLabel.Text = "Access token OK";
                OAuthAvailableLabel.ForeColor = Color.Green;
            }
            else
            {
                OAuthAvailableLabel.Text = "Access token NOK";
                OAuthAvailableLabel.ForeColor = Color.Red;
            }
        }

        private void OAuthButton_Click(object sender, EventArgs e)
        {
            OAuthWindow oAuth = new OAuthWindow();
            oAuth.Go(twitch);
            configHandler.SetAccessToken(oAuth.AuthId);
            Reset();
            ListChannels();
        }

        private void RefreshChannelsButton_Click(object sender, EventArgs e)
        {
            Reset();
            ListChannels();
        }

        private void Reset()
        {
            channels.Clear();
            ChannelGridView.DataSource = null;
            ChannelGridView.Rows.Clear();
            ChannelGridView.Refresh();
            SetAccessTokenLabel();
        }

        private async void ListChannels()
        {
            if (!configHandler.HasAccessToken)
            {
                return;
            }

            TwitchRootResponse inChannels = null;
            try
            {
                inChannels = await twitch.GetLiveChannels(configHandler.Config.AccessToken);
            }
            catch (Exception e)
            {
                ChangeStatusText("Error: Failed to get live channels from Twitch. Exception: " + e.ToString());
                return;
            }

            if (inChannels?.streams == null)
            {
                ChangeStatusText("Error: Could not get live channels. Try to do OAuth again", TimeSpan.FromMilliseconds(2000));
                return;
            }

            for (int i = 0; i < inChannels.streams.Count; i++)
            {
                var chan = inChannels.streams[i];
                channels.Add(new TwitchChannel(chan.channel.name, chan.channel.url));
                TimeSpan runtime = DateTime.Now.ToUniversalTime() - chan.created_at;
                ChannelGridView.Rows.Add(chan.channel.name, chan.channel.status, chan.game, chan.viewers, new DateTime(runtime.Ticks).ToString("H\\h mm\\m"));
                try
                {
                    Bitmap bitmap = await GetBmp(chan.preview.medium);
                    if (bitmap != null)
                    {
                        ChannelGridView.Rows[i].Cells[5].Value = bitmap;
                    }
                }
                catch { }
            }
        }

        private async Task<Bitmap> GetBmp(string url)
        {
            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(url);
            myRequest.Method = "GET";
            Bitmap bmp = null;
            using (var myResponse = (HttpWebResponse)await myRequest.GetResponseAsync())
            {
                bmp = new Bitmap(myResponse.GetResponseStream());
            }
            return bmp;
        }

        private void ChannelGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var senderGrid = (DataGridView)sender;

            if (senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn &&
                e.RowIndex >= 0)
            {
                var channel = channels[e.RowIndex];
                StartStream(channel.Url);
            }
        }

        private List<string> GetQualityFromRadioButtons()
        {
            if (RadioQualityHigh.Checked)
            {
                return QualityOptions.HighQuality;
            }
            if (RadioQualityMedium.Checked)
            {
                return QualityOptions.MediumQuality;
            }
            if (RadioQualityLow.Checked)
            {
                return QualityOptions.LowQuality;
            }
            return QualityOptions.HighQuality;
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
                ResetStatusText();
            }
            else
            {
                ChangeStatusText($"Running processes: {runningProcesses }");
            }
        }

        private void ChangeStatusText(string status, TimeSpan? time = null)
        {
            if (statusTextTimer != null && statusTextTimer.Enabled && time != null)
            {
                statusTextTimer.Stop();
            }

            if (statusStrip1.InvokeRequired)
            {
                statusStrip1.Invoke(new MethodInvoker(() => toolStripStatusLabel1.Text = status));
            }
            else
            {
                toolStripStatusLabel1.Text = status;
            }

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

        private void ResetStatusText()
        {
            ChangeStatusText($"");
        }

        private void LaunchChannelButton_Click(object sender, EventArgs e)
        {
            string url = textBox_URL.Text;
            if (!url.Contains("twitch.tv"))
            {
                ChangeStatusText("Invalid URL - Should be formatted as: http://www.twitch.tv/channel");
                return;
            }
            if (url.Equals("http://www.twitch.tv/channel")) // Initial example text - ignore
            {
                return;
            }

            StartStream(new Uri(url));
        }

        private void StartStream(Uri url)
        {
            List<string> qualityOptions = GetQualityFromRadioButtons();
            streamService.StartStream(url, qualityOptions);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if ((ModifierKeys & Keys.Shift) == 0)
            {
                string initLocation = Properties.Settings.Default.InitialLocation;
                Point newLocation = new Point(0, 0);
                Size newSize = Size;
                if (!string.IsNullOrWhiteSpace(initLocation))
                {
                    string[] parts = initLocation.Split(',');
                    if (parts.Length >= 2)
                    {
                        newLocation = new Point(int.Parse(parts[0]), int.Parse(parts[1]));
                    }
                    if (parts.Length >= 4)
                    {
                        newSize = new Size(int.Parse(parts[2]), int.Parse(parts[3]));
                    }
                }
                Size = newSize;
                Location = newLocation;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if ((ModifierKeys & Keys.Shift) == 0)
            {
                Point location = Location;
                Size size = Size;
                if (WindowState != FormWindowState.Normal)
                {
                    location = RestoreBounds.Location;
                    size = RestoreBounds.Size;
                }
                string initLocation = string.Join(",", location.X, location.Y, size.Width, size.Height);
                Properties.Settings.Default.InitialLocation = initLocation;
                Properties.Settings.Default.Save();
            }
        }
    }
}
