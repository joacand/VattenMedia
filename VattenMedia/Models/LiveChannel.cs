using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media.Imaging;

namespace VattenMedia.Models
{
    internal class LiveChannel : INotifyPropertyChanged
    {
        public string Name { get; }
        public string Title { get; }
        public string Game { get; }
        public int Viewers { get; }
        public string RunTime { get; }
        public Uri BitmapUrl { get; }
        public Uri Url { get; }
        public BitmapImage Image { get; private set; }
        public string ChannelId { get; set; }
        public bool IsFavorited { get; set; }

        public string Description => $"{Name} ({Viewers} viewers) - {Title}";

        public LiveChannel(
            string name,
            string title,
            string game,
            int viewers,
            string runTime,
            string bitmapUrl,
            Uri url,
            string channelId,
            bool isFavorited)
        {
            Name = name;
            Title = title;
            Game = game;
            Viewers = viewers;
            RunTime = runTime;
            BitmapUrl = CreateBitmapUrl(bitmapUrl);
            Url = url;
            ChannelId = channelId;
            IsFavorited = isFavorited;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private static Uri CreateBitmapUrl(string bitmapUrl)
        {
            if (string.IsNullOrWhiteSpace(bitmapUrl)) { return null; }
            bitmapUrl = bitmapUrl.Replace("{width}", "400").Replace("{height}", "300");
            return new Uri(bitmapUrl);
        }

        public LiveChannel(Core.Entities.LiveChannel liveChannel, bool isFavorited) : this(
                  liveChannel.Name,
                  liveChannel.Title,
                  liveChannel.Game,
                  liveChannel.Viewers,
                  liveChannel.RunTime,
                  liveChannel.BitmapUrl.ToString(),
                  liveChannel.Url,
                  liveChannel.ChannelId,
                  isFavorited)
        { }

        public void LoadImage()
        {
            var image = GetImage();
            image.Freeze(); // To allow application current dispatcher to use object created in a different thread
            Application.Current.Dispatcher.Invoke(() =>
            {
                Image = image;
            });
        }

        private BitmapImage GetImage()
        {
            var myRequest = (HttpWebRequest)WebRequest.Create(BitmapUrl);
            myRequest.Method = "GET";
            Bitmap bitmap = null;
            using (var myResponse = (HttpWebResponse)myRequest.GetResponse())
            {
                bitmap = new Bitmap(myResponse.GetResponseStream());
            }

            using MemoryStream memory = new MemoryStream();
            bitmap.Save(memory, ImageFormat.Bmp);
            bitmap.Dispose();
            memory.Position = 0;
            var bitmapimage = new BitmapImage();
            bitmapimage.BeginInit();
            bitmapimage.StreamSource = memory;
            bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapimage.EndInit();
            return bitmapimage;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
