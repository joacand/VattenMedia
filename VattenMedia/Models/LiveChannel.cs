using System;
using System.Drawing;
using System.Net;
using System.Windows.Media.Imaging;
using System.Drawing.Imaging;
using System.IO;

namespace VattenMedia.Models
{
    internal class LiveChannel
    {
        public string Name { get; }
        public string Title { get; }
        public string Game { get; }
        public int Viewers { get; }
        public string RunTime { get; }
        public Uri BitmapUrl { get; }
        public Uri Url { get; }
        public BitmapImage Image { get; private set; }
        public bool IsFavorited { get; set; }

        public string Description => $"{Name} ({Viewers} viewers) - {Title}";

        public LiveChannel(
            string name,
            string title,
            string game,
            int viewers,
            string runTime,
            string bitmapUrl,
            string url,
            bool isFavorited)
        {
            Name = name;
            Title = title;
            Game = game;
            Viewers = viewers;
            RunTime = runTime;
            BitmapUrl = string.IsNullOrWhiteSpace(bitmapUrl) ? null : new Uri(bitmapUrl);
            Url = string.IsNullOrWhiteSpace(url) ? null : new Uri(url);
            Image = GetImage();
            IsFavorited = isFavorited;
        }

        public LiveChannel(Core.Entities.LiveChannel liveChannel, bool isFavorited) : this(
                  liveChannel.Name,
                  liveChannel.Title,
                  liveChannel.Game,
                  liveChannel.Viewers,
                  liveChannel.RunTime,
                  liveChannel.BitmapUrl.ToString(),
                  liveChannel.Url.ToString(),
                  isFavorited)
        { }

        private BitmapImage GetImage()
        {
            var myRequest = (HttpWebRequest)WebRequest.Create(BitmapUrl);
            myRequest.Method = "GET";
            Bitmap bitmap = null;
            using (var myResponse = (HttpWebResponse)myRequest.GetResponse())
            {
                bitmap = new Bitmap(myResponse.GetResponseStream());
            }

            using (MemoryStream memory = new MemoryStream())
            {
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
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
